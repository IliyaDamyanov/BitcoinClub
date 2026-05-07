using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BitcoinClub.Data;
using BitcoinClub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BitcoinClub.Importing
{
    public sealed class SubscriptionCsvImporter
    {
        private readonly ApplicationDbContext _db;
        private readonly GoogleSheetsSubscriptionCsvReader _reader;
        private readonly SubscriptionImportMapper _mapper;

        public SubscriptionCsvImporter(
            ApplicationDbContext db,
            GoogleSheetsSubscriptionCsvReader reader,
            SubscriptionImportMapper mapper)
        {
            _db = db;
            _reader = reader;
            _mapper = mapper;
        }

        public async Task<SubscriptionImportResult> ImportAsync(string csvPath, bool dryRun = false, CancellationToken cancellationToken = default)
        {
            var rows = _reader.Read(csvPath);
            var result = new SubscriptionImportResult { RowsProcessed = rows.Count };

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row.FullName) && string.IsNullOrWhiteSpace(row.Email) && string.IsNullOrWhiteSpace(row.DiscordNickname))
                {
                    result.RowsSkipped++;
                    continue;
                }

                var user = await UpsertUserAsync(row, result, cancellationToken);
                var subscription = await UpsertSubscriptionAsync(row, user, result, cancellationToken);
                await UpsertProfileAsync(row, user, result, cancellationToken);
                await UpsertPaymentsAsync(row, user, subscription, result, cancellationToken);
            }

            if (!dryRun)
            {
                await _db.SaveChangesAsync(cancellationToken);
            }

            return result;
        }

        private async Task<IdentityUser> UpsertUserAsync(
            GoogleSheetsSubscriptionRow row,
            SubscriptionImportResult result,
            CancellationToken cancellationToken)
        {
            var email = _mapper.BuildImportedEmail(row);
            var normalizedEmail = email.ToUpperInvariant();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

            if (user is null)
            {
                user = new IdentityUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = email,
                    NormalizedEmail = normalizedEmail,
                    UserName = email,
                    NormalizedUserName = normalizedEmail,
                    EmailConfirmed = !string.IsNullOrWhiteSpace(row.Email),
                    PhoneNumber = string.IsNullOrWhiteSpace(row.Phone) ? null : row.Phone
                };
                _db.Users.Add(user);
                result.UsersUpserted++;
                return user;
            }

            var changed = false;
            if (!string.IsNullOrWhiteSpace(row.Phone) && user.PhoneNumber != row.Phone)
            {
                user.PhoneNumber = row.Phone;
                changed = true;
            }

            if (changed)
            {
                result.UsersUpserted++;
            }

            return user;
        }

        private async Task<Subscription> UpsertSubscriptionAsync(
            GoogleSheetsSubscriptionRow row,
            IdentityUser user,
            SubscriptionImportResult result,
            CancellationToken cancellationToken)
        {
            var subscription = await _db.Subscriptions.FirstOrDefaultAsync(s => s.UserId == user.Id, cancellationToken);
            var expirationDate = row.ExpirationDate ?? DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
            var changed = false;

            if (subscription is null)
            {
                subscription = new Subscription
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    ExpirationDate = expirationDate,
                    LastPaymentDate = row.LastPaymentDate,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Subscriptions.Add(subscription);
                result.SubscriptionsUpserted++;
                return subscription;
            }

            if (subscription.ExpirationDate != expirationDate)
            {
                subscription.ExpirationDate = expirationDate;
                changed = true;
            }

            if (subscription.LastPaymentDate != row.LastPaymentDate)
            {
                subscription.LastPaymentDate = row.LastPaymentDate;
                changed = true;
            }

            if (changed)
            {
                result.SubscriptionsUpserted++;
            }

            return subscription;
        }

        private async Task UpsertProfileAsync(
            GoogleSheetsSubscriptionRow row,
            IdentityUser user,
            SubscriptionImportResult result,
            CancellationToken cancellationToken)
        {
            var profile = await _db.ImportedMemberProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id, cancellationToken);
            if (profile is null)
            {
                profile = new ImportedMemberProfile { Id = Guid.NewGuid(), UserId = user.Id };
                _db.ImportedMemberProfiles.Add(profile);
                result.ProfilesUpserted++;
            }

            profile.FullName = row.FullName;
            profile.DiscordNickname = row.DiscordNickname;
            profile.Position = row.Position;
            profile.MemberSince = row.MemberSince;
            profile.TotalContributionsRaw = row.TotalContributionsRaw;
            profile.VolunteerInterests = row.VolunteerInterests;
            profile.StreetAddress = row.StreetAddress;
            profile.City = row.City;
            profile.Region = row.Region;
            profile.PostalCode = row.PostalCode;
            profile.SecondaryEmail = row.SecondaryEmail;
            profile.Notes = row.Notes;
            profile.UpdatedAt = DateTime.UtcNow;
        }

        private async Task UpsertPaymentsAsync(
            GoogleSheetsSubscriptionRow row,
            IdentityUser user,
            Subscription subscription,
            SubscriptionImportResult result,
            CancellationToken cancellationToken)
        {
            foreach (var paymentEntry in row.Payments.OrderBy(p => p.PaidMonth))
            {
                var providerId = _mapper.BuildPaymentProviderId(row, paymentEntry);
                var exists = await _db.Payments.AnyAsync(
                    p => p.Provider == "spreadsheet" && p.ProviderPaymentId == providerId,
                    cancellationToken);

                if (exists)
                {
                    continue;
                }

                _db.Payments.Add(new Payment
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    SubscriptionId = subscription.Id,
                    Provider = "spreadsheet",
                    ProviderPaymentId = providerId,
                    AmountSats = 0,
                    PaymentRequest = null,
                    Status = paymentEntry.IsFree ? "free" : "completed",
                    CreatedAt = paymentEntry.PaidMonth,
                    PaidAt = paymentEntry.PaidMonth
                });
                result.PaymentsUpserted++;
            }
        }
    }

    public sealed class SubscriptionImportResult
    {
        public int RowsProcessed { get; set; }

        public int RowsSkipped { get; set; }

        public int UsersUpserted { get; set; }

        public int SubscriptionsUpserted { get; set; }

        public int ProfilesUpserted { get; set; }

        public int PaymentsUpserted { get; set; }
    }
}
