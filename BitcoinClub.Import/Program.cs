using System;
using System.IO;
using System.Threading.Tasks;
using BitcoinClub.Data;
using BitcoinClub.Importing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BitcoinClub.Import
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.Combine("BitcoinClub", "appsettings.json"), optional: false)
                .AddJsonFile(Path.Combine("BitcoinClub", "appsettings.Development.json"), optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.Error.WriteLine("DefaultConnection is missing.");
                return 1;
            }

            var csvPath = args.Length > 0 ? args[0] : Path.Combine("TestData", "INCOMES AND EXPENSES.csv");
            if (!File.Exists(csvPath))
            {
                Console.Error.WriteLine($"CSV file not found: {csvPath}");
                return 1;
            }

            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(connectionString));

            services.AddScoped<GoogleSheetsSubscriptionCsvReader>();
            services.AddScoped<SubscriptionImportMapper>();
            services.AddScoped<SubscriptionCsvImporter>();

            var sp = services.BuildServiceProvider();

            await using (var scope = sp.CreateAsyncScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await db.Database.MigrateAsync();

                var importer = scope.ServiceProvider.GetRequiredService<SubscriptionCsvImporter>();
                var result = await importer.ImportAsync(csvPath);

                Console.WriteLine($"Rows processed: {result.RowsProcessed}");
                Console.WriteLine($"Users upserted: {result.UsersUpserted}");
                Console.WriteLine($"Subscriptions upserted: {result.SubscriptionsUpserted}");
                Console.WriteLine($"Rows skipped: {result.RowsSkipped}");
            }

            return 0;
        }
    }
}
