using System.Linq;
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
                return View("NoSubscription");
            }

            var payments = await _db.Payments
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PaymentHistoryItem
                {
                    CreatedAt = p.CreatedAt,
                    AmountSats = p.AmountSats,
                    Status = p.Status,
                    PaidAt = p.PaidAt
                })
                .ToListAsync();

            var vm = new MembershipDashboardViewModel
            {
                ExpirationDate = subscription.ExpirationDate,
                LastPaymentDate = subscription.LastPaymentDate,
                PaymentHistory = payments
            };

            return View(vm);
        }
    }
}
