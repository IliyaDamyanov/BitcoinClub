using BitcoinClub.Data;
using BitcoinClub.Infrastructure.Auth;
using BitcoinClub.Infrastructure.Auth.Providers;
using BitcoinClub.Infrastructure.Database;
using BitcoinClub.Infrastructure.Files;
using BitcoinClub.Infrastructure.Payments;
using BitcoinClub.Services.CalendarEvents;
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

                var useSqlite = builder.Configuration.GetValue<bool>("UseSqlite");
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

                if (useSqlite)
                {
                    builder.Services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlite(connectionString));
                }
                else
                {
                    ConnectionStringValidator.ValidatePostgresConnectionString(connectionString);
                    builder.Services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseNpgsql(connectionString));
                }
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

            builder.Services.AddGlowPayPayments(builder.Configuration);

            builder.Services.AddSingleton<IUploadPathValidator, UploadPathValidator>();
            builder.Services.AddScoped<IFileUploadService, FileUploadService>();

            builder.Services.AddScoped<ILandingPageContentService, LandingPageContentService>();

            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton(TimeProvider.System);
            builder.Services.AddHttpClient<ICalendarEventsService, CalendarEventsService>();

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

            app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

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
