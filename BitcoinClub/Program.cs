using BitcoinClub.Data;
using BitcoinClub.Infrastructure.Auth;
using BitcoinClub.Infrastructure.Auth.Providers;
using BitcoinClub.Infrastructure.Database;
using BitcoinClub.Infrastructure.Files;
using BitcoinClub.Infrastructure.Payments;
using BitcoinClub.Infrastructure.Social;
using BitcoinClub.Services.Landing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Globalization;

namespace BitcoinClub
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("logs/bitcoinclub-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            try
            {
                Log.Information("Starting BitcoinClub application");

                var builder = WebApplication.CreateBuilder(args);
                builder.Host.UseSerilog();

                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
                ConnectionStringValidator.ValidatePostgresConnectionString(connectionString);

                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(connectionString));
                builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.User.RequireUniqueEmail = true;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, DefaultRoleUserClaimsPrincipalFactory>();

            builder.Services.AddScoped<IAuthProvider, EmailPasswordAuthProvider>();

            builder.Services.AddBreezPayments(builder.Configuration);

            builder.Services.AddSingleton<IUploadPathValidator, UploadPathValidator>();
            builder.Services.AddScoped<IFileUploadService, FileUploadService>();

            builder.Services.AddSingleton<ISocialMediaPublisher, FacebookPublisher>();
            builder.Services.AddSingleton<ISocialMediaPublisher, InstagramPublisher>();
            builder.Services.AddSingleton<ISocialMediaPublisher, ThreadsPublisher>();
            builder.Services.AddSingleton<ISocialMediaPublisher, TwitterPublisher>();
            builder.Services.AddSingleton<ISocialMediaPublisher, NostrPublisher>();

            builder.Services.AddScoped<ISocialMediaPublishManager, SocialMediaPublishManager>();

            builder.Services.AddScoped<ILandingPageContentService, LandingPageContentService>();

            // Set resources path so view and controller localization looks in the Resources/ folder
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.AddControllersWithViews()
                .AddViewLocalization();

            var app = builder.Build();

            await RoleSeeder.SeedAsync(app.Services);

            var supportedCultures = new[] { new CultureInfo("bg"), new CultureInfo("en") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("bg"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }
    }
}
