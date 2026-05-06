using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO;

namespace BitcoinClub.Importing
{
    public class GoogleSheetsSubscriptionCsvReader
    {
        public IReadOnlyList<GoogleSheetsSubscriptionRow> Read(string path)
        {
            var rows = new List<GoogleSheetsSubscriptionRow>();

            using var parser = new TextFieldParser(path);
            parser.SetDelimiters(",");
            parser.HasFieldsEnclosedInQuotes = true;

            var lineIndex = 0;
            string[]? header = null;

            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields() ?? Array.Empty<string>();
                lineIndex++;

                if (lineIndex == 1)
                {
                    continue;
                }

                if (header is null)
                {
                    header = fields;
                    continue;
                }

                if (fields.Length == 0)
                {
                    continue;
                }

                var fullName = Get(fields, 0);
                var discord = Get(fields, 1);
                var email = NormalizeEmail(Get(fields, 2));
                var position = Get(fields, 3);
                var totalContributions = Get(fields, 4);
                var memberSince = ParseYear(Get(fields, 5));
                var volunteerInterests = Get(fields, 8);
                var phone = Get(fields, 9);
                var streetAddress = Get(fields, 10);
                var city = Get(fields, 11);
                var region = Get(fields, 12);
                var postalCode = Get(fields, 13);
                var secondaryEmail = NormalizeEmail(Get(fields, 14)) ?? string.Empty;
                var notes = Get(fields, 15);

                var payments = ParsePayments(header, fields);
                var lastPayment = payments.Count == 0 ? (DateTime?)null : payments[^1].PaidMonth;
                var expiration = lastPayment?.AddMonths(1);

                if (string.IsNullOrWhiteSpace(fullName) && string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(discord))
                {
                    continue;
                }

                rows.Add(new GoogleSheetsSubscriptionRow(
                    fullName,
                    discord,
                    email,
                    position,
                    totalContributions,
                    memberSince,
                    volunteerInterests,
                    phone,
                    streetAddress,
                    city,
                    region,
                    postalCode,
                    secondaryEmail,
                    notes,
                    expiration,
                    lastPayment,
                    payments));
            }

            return rows;
        }

        private static IReadOnlyList<GoogleSheetsPaymentEntry> ParsePayments(string[] header, string[] fields)
        {
            var payments = new List<GoogleSheetsPaymentEntry>();

            for (var i = 0; i < header.Length && i < fields.Length; i++)
            {
                var h = header[i]?.Trim();
                if (string.IsNullOrWhiteSpace(h))
                {
                    continue;
                }

                if (!TryParseMonthYearHeader(h, out var month))
                {
                    continue;
                }

                var cell = (fields[i] ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(cell))
                {
                    continue;
                }

                if (cell.Equals("FREE", StringComparison.OrdinalIgnoreCase) || cell.StartsWith("BGN", StringComparison.OrdinalIgnoreCase))
                {
                    payments.Add(new GoogleSheetsPaymentEntry(
                        month,
                        cell,
                        ParseBgnAmount(cell),
                        cell.Equals("FREE", StringComparison.OrdinalIgnoreCase)));
                }
            }

            return payments;
        }

        private static bool TryParseMonthYearHeader(string headerCell, out DateTime month)
        {
            month = default;

            headerCell = headerCell.Trim();

            var match = Regex.Match(headerCell, @"(?<month>\d{1,2})\s*/\s*(?<year>\d{4})");
            if (match.Success
                && int.TryParse(match.Groups["month"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedMonth)
                && int.TryParse(match.Groups["year"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedYear)
                && parsedMonth >= 1
                && parsedMonth <= 12)
            {
                month = new DateTime(parsedYear, parsedMonth, 1, 0, 0, 0, DateTimeKind.Utc);
                return true;
            }

            return false;
        }

        private static decimal? ParseBgnAmount(string cell)
        {
            var match = Regex.Match(cell, @"(?<amount>\d+(?:\.\d+)?)");
            if (!match.Success)
            {
                return null;
            }

            if (decimal.TryParse(match.Groups["amount"].Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
            {
                return amount;
            }

            return null;
        }

        private static DateTime? ParseYear(string raw)
        {
            raw = (raw ?? string.Empty).Trim();
            if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var year) && year >= 2000 && year <= 2100)
            {
                return new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }

            return null;
        }

        private static string Get(string[] fields, int index)
        {
            if (index < 0 || index >= fields.Length)
            {
                return string.Empty;
            }

            return (fields[index] ?? string.Empty).Trim();
        }

        private static string? NormalizeEmail(string raw)
        {
            raw = (raw ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(raw))
            {
                return null;
            }

            return raw.ToLowerInvariant();
        }
    }
}
