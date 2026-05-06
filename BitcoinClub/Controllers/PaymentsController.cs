using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BitcoinClub.Data;
using BitcoinClub.Infrastructure.Payments;
using BitcoinClub.Models;
using BitcoinClub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BitcoinClub.Controllers
{
    [Authorize]
    public sealed class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IPaymentService _payments;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(ApplicationDbContext db, IPaymentService payments, ILogger<PaymentsController> logger)
        {
            _db = db;
            _payments = payments;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Status()
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

            var vm = new PaymentStatusViewModel
            {
                ExpirationDate = subscription.ExpirationDate,
                LastPaymentDate = subscription.LastPaymentDate
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult Initiate()
        {
            return View(new PaymentInitiateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Initiate(PaymentInitiateViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            if (model.AmountSats <= 0)
            {
                ModelState.AddModelError(nameof(model.AmountSats), "Amount must be greater than 0.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _logger.LogInformation("Initiating payment for user {UserId}, amount {AmountSats} sats", userId, model.AmountSats);
            var init = await _payments.InitiateMembershipPaymentAsync(userId, model.AmountSats, model.Description);

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SubscriptionId = init.SubscriptionId,
                Provider = "glow-pay",
                ProviderPaymentId = init.PaymentId,
                AmountSats = init.AmountSats,
                PaymentRequest = init.PaymentRequest,
                Status = "initiated",
                CreatedAt = DateTime.UtcNow
            };

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            model.SubscriptionId = init.SubscriptionId;
            model.PaymentRecordId = payment.Id;
            model.PaymentId = init.PaymentId;
            model.PaymentRequest = init.PaymentRequest;
            model.PaymentUrl = init.PaymentUrl;
            model.ExpiresAt = init.ExpiresAt;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(Guid subscriptionId, Guid paymentRecordId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            if (subscriptionId == Guid.Empty || paymentRecordId == Guid.Empty)
            {
                return BadRequest();
            }

            var payment = await _db.Payments.SingleOrDefaultAsync(p => p.Id == paymentRecordId && p.UserId == userId && p.SubscriptionId == subscriptionId);
            if (payment is null)
            {
                return NotFound();
            }

            _logger.LogInformation("Verifying payment {PaymentId} for subscription {SubscriptionId}", payment.ProviderPaymentId, subscriptionId);
            var verify = await _payments.VerifyPaymentAsync(subscriptionId, payment.ProviderPaymentId);
            if (!verify.IsPaid)
            {
                _logger.LogInformation("Payment {PaymentId} not yet paid, marking pending", payment.ProviderPaymentId);
                payment.Status = "pending";
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Initiate));
            }

            _logger.LogInformation("Payment {PaymentId} confirmed paid at {PaidAt}", payment.ProviderPaymentId, verify.PaidAt);
            payment.Status = "paid";
            payment.PaidAt = verify.PaidAt?.UtcDateTime;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Status));
        }
    }
}
