using System;

namespace BitcoinClub.Importing
{
    public sealed record GoogleSheetsSubscriptionRow(
        string FullName,
        string DiscordNickname,
        string? Email,
        string Position,
        string TotalContributionsRaw,
        DateTime? MemberSince,
        string VolunteerInterests,
        string Phone,
        string StreetAddress,
        string City,
        string Region,
        string PostalCode,
        string SecondaryEmail,
        string Notes,
        DateTime? ExpirationDate,
        DateTime? LastPaymentDate,
        IReadOnlyList<GoogleSheetsPaymentEntry> Payments);

    public sealed record GoogleSheetsPaymentEntry(
        DateTime PaidMonth,
        string RawValue,
        decimal? AmountBgn,
        bool IsFree);
}
