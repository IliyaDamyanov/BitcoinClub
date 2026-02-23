using System.Diagnostics;
using System.Globalization;
using BitcoinClub.Models;
using BitcoinClub.Services.Landing;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace BitcoinClub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILandingPageContentService _landingPageContentService;

        public HomeController(ILogger<HomeController> logger, ILandingPageContentService landingPageContentService)
        {
            _logger = logger;
            _landingPageContentService = landingPageContentService;
        }

        public IActionResult Index([FromQuery] string? lang)
        {
            // Keep the existing query-string toggle, but map it to real cultures for resx localization.
            var culture = string.Equals(lang, "EN", StringComparison.OrdinalIgnoreCase) ? "en" : "bg";

            if (HttpContext != null)
            {
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            }

            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);

            var vm = _landingPageContentService.Get(lang);
            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
