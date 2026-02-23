using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Xunit;

namespace BitcoinClub.Tests.Localization;

public sealed class RegisterViewLocalizationTests
{
    [Fact]
    public void ViewResources_WhenCultureIsBg_ResolvesBulgarianTitle()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        var sp = services.BuildServiceProvider();

        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("bg");

            var localizerFactory = sp.GetRequiredService<IStringLocalizerFactory>();

            var localizer = localizerFactory.Create("Views.Account.Register", "BitcoinClub");

            Assert.Equal("Регистрация", localizer["Register_Title"].Value);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }
}
