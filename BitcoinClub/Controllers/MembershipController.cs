using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BitcoinClub.Data;
using BitcoinClub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BitcoinClub.Controllers
{
    [Authorize]
    public class MembershipController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MembershipController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var subscription = await _db.Subscriptions.SingleOrDefaultAsync(s => s.UserId == userId);
            if (subscription is null)
            {
                return NotFound();
            }

            var vm = new MembershipDashboardViewModel
            {
                ExpirationDate = subscription.ExpirationDate,
                LastPaymentDate = subscription.LastPaymentDate
            };

            return View(vm);
        }
    }
}
