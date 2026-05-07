using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BitcoinClub.Importing
{
    public sealed class SubscriptionImportMapper
    {
        public string BuildUserKey(GoogleSheetsSubscriptionRow row)
        {
            if (!string.IsNullOrWhiteSpace(row.Email))
            {
                return row.Email.Trim().ToLowerInvariant();
            }

            if (!string.IsNullOrWhiteSpace(row.DiscordNickname))
            {
                return $"discord:{NormalizeIdentifier(row.DiscordNickname)}";
            }

            return $"member:{NormalizeIdentifier(row.FullName)}";
        }

        public string BuildImportedEmail(GoogleSheetsSubscriptionRow row)
        {
            return string.IsNullOrWhiteSpace(row.Email)
                ? $"{BuildUserKey(row)}@import.bitcoinclub.local"
                : row.Email.Trim().ToLowerInvariant();
        }

        public string BuildPaymentProviderId(GoogleSheetsSubscriptionRow row, GoogleSheetsPaymentEntry payment)
        {
            return string.Create(CultureInfo.InvariantCulture, $"spreadsheet:{BuildUserKey(row)}:{payment.PaidMonth:yyyyMM}");
        }

        private static string NormalizeIdentifier(string value)
        {
            var cleaned = new StringBuilder();
            foreach (var ch in value.Trim().ToLowerInvariant())
            {
                if (char.IsLetterOrDigit(ch))
                {
                    cleaned.Append(ch);
                    continue;
                }

                if (cleaned.Length > 0 && cleaned[^1] != '-')
                {
                    cleaned.Append('-');
                }
            }

            var normalized = cleaned.ToString().Trim('-');
            return string.IsNullOrWhiteSpace(normalized) ? "unknown" : normalized;
        }
    }
}
