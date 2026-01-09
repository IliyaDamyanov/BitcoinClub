using BitcoinClub.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BitcoinClub.Controllers
{
    [RequireRole(RoleNames.Member)]
    public class MembersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
