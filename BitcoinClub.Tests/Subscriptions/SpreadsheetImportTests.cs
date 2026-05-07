using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BitcoinClub.Data;
using BitcoinClub.Importing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BitcoinClub.Tests.Subscriptions
{
    public sealed class SpreadsheetImportTests
    {
        [Fact]
        public void Reader_maps_member_profile_and_monthly_payments()
        {
            var path = WriteFixtureCsv();
            var rows = new GoogleSheetsSubscriptionCsvReader().Read(path);

            var row = Assert.Single(rows);
            Assert.Equal("Test Member", row.FullName);
            Assert.Equal("tester", row.DiscordNickname);
            Assert.Equal("test@example.com", row.Email);
            Assert.Equal("Volunteer", row.Position);
            Assert.Equal(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), row.MemberSince);
            Assert.Equal(new DateTime(2024, 12, 1, 0, 0, 0, DateTimeKind.Utc), row.LastPaymentDate);
            Assert.Equal(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), row.ExpirationDate);
            Assert.Equal(2, row.Payments.Count);
            Assert.Contains(row.Payments, p => p.PaidMonth == new DateTime(2024, 9, 1, 0, 0, 0, DateTimeKind.Utc) && p.AmountBgn == 105m);
            Assert.Contains(row.Payments, p => p.PaidMonth == new DateTime(2024, 12, 1, 0, 0, 0, DateTimeKind.Utc) && p.IsFree);
        }

        [Fact]
        public async Task Importer_is_idempotent_for_users_subscriptions_profiles_and_payments()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var path = WriteFixtureCsv();

            await using var db = new ApplicationDbContext(options);
            var importer = new SubscriptionCsvImporter(
                db,
                new GoogleSheetsSubscriptionCsvReader(),
                new SubscriptionImportMapper());

            var first = await importer.ImportAsync(path);
            var second = await importer.ImportAsync(path);

            Assert.Equal(1, first.UsersUpserted);
            Assert.Equal(1, first.SubscriptionsUpserted);
            Assert.Equal(1, first.ProfilesUpserted);
            Assert.Equal(2, first.PaymentsUpserted);
            Assert.Equal(0, second.UsersUpserted);
            Assert.Equal(0, second.SubscriptionsUpserted);
            Assert.Equal(0, second.PaymentsUpserted);
            Assert.Equal(1, db.Users.Count());
            Assert.Equal(1, db.Subscriptions.Count());
            Assert.Equal(1, db.ImportedMemberProfiles.Count());
            Assert.Equal(2, db.Payments.Count());
        }

        private static string WriteFixtureCsv()
        {
            var path = Path.Combine(Path.GetTempPath(), $"bitcoinclub-import-{Guid.NewGuid():N}.csv");
            File.WriteAllText(path, string.Join(Environment.NewLine, new[]
            {
                "BITCOIN CLUB BG,,,,,,,,,,,,,,,,,,",
                "Имена,Дискорд никнейм,Email,Позиция,Членски внос Платени,Член от (година),,Месечна вноска 9/2024,Интереси на доброволец,Телефон,Улица и номер,Град,Област,Пощенски код,Имейл адрес,Бележки,10/2024,11/2024,12/2024,1/2025",
                "Test Member,tester,test@example.com,Volunteer,BGN lev210.00,2024,,BGN105.00,events,+3591,Street 1,Sofia,Sofia,1000,second@example.com,fixture note,,,FREE"
            }));
            return path;
        }
    }
}
