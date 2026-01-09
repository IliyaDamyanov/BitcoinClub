using BitcoinClub.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BitcoinClub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleNames.Admin)]
    public sealed class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
