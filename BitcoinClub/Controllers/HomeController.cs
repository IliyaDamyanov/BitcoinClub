using System.Diagnostics;
using BitcoinClub.Models;
using BitcoinClub.Services.Events;
using BitcoinClub.Services.Landing;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace BitcoinClub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILandingPageContentService _landingPageContentService;
        private readonly IEventsService _eventsService;

        public HomeController(ILogger<HomeController> logger, ILandingPageContentService landingPageContentService, IEventsService eventsService)
        {
            _logger = logger;
            _landingPageContentService = landingPageContentService;
            _eventsService = eventsService;
        }

        public async Task<IActionResult> Index([FromQuery] string? lang)
        {
            // When a lang toggle is clicked, persist the choice as a cookie and redirect
            // to the clean URL. This ensures the UseRequestLocalization middleware reads
            // the correct cookie on the next request and sets CultureInfo.CurrentUICulture
            // for the entire pipeline—including IStringLocalizer calls in partial views.
            if (!string.IsNullOrEmpty(lang))
            {
                var culture = string.Equals(lang, "EN", StringComparison.OrdinalIgnoreCase) ? "en" : "bg";
                if (HttpContext != null)
                {
                    Response.Cookies.Append(
                        CookieRequestCultureProvider.DefaultCookieName,
                        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
                }
                return RedirectToAction(nameof(Index));
            }

            var vm = _landingPageContentService.Get();
            vm.UpcomingEvents = (await _eventsService.GetUpcomingAsync()).ToArray();
            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
