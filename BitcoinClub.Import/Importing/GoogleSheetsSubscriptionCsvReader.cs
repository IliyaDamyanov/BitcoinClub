using System;
using System.Collections.Generic;
using System.Globalization;
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

                if (lineIndex <= 2)
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
                var memberSince = ParseYear(Get(fields, 5));

                var (lastPayment, expiration) = ParsePayments(header, fields);

                if (string.IsNullOrWhiteSpace(fullName) && string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(discord))
                {
                    continue;
                }

                rows.Add(new GoogleSheetsSubscriptionRow(
                    fullName,
                    discord,
                    email,
                    position,
                    memberSince,
                    expiration,
                    lastPayment));
            }

            return rows;
        }

        private static (DateTime? lastPayment, DateTime? expiration) ParsePayments(string[] header, string[] fields)
        {
            DateTime? lastPaid = null;
            DateTime? expiration = null;

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
                    lastPaid = month;
                }
            }

            if (lastPaid.HasValue)
            {
                expiration = lastPaid.Value.AddMonths(1);
            }

            return (lastPaid, expiration);
        }

        private static bool TryParseMonthYearHeader(string headerCell, out DateTime month)
        {
            month = default;

            headerCell = headerCell.Trim();

            if (DateTime.TryParseExact(headerCell, "M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            {
                month = new DateTime(parsed.Year, parsed.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                return true;
            }

            return false;
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
