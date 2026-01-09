using Microsoft.AspNetCore.Mvc;

namespace BitcoinClub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public sealed class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
