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
        services.AddLocalization();

        var sp = services.BuildServiceProvider();

        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("bg");

            var localizerFactory = sp.GetRequiredService<IStringLocalizerFactory>();

            // View localization looks for resources under: Resources/Views/Account/Register.*.resx
            // The base name is: Views.Account.Register
            var localizer = localizerFactory.Create("Views.Account.Register", "BitcoinClub");

            Assert.Equal("Регистрация", localizer["Register_Title"].Value);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }
}
