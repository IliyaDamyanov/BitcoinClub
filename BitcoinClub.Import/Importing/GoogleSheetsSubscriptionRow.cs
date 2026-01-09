using System;

namespace BitcoinClub.Importing
{
    public sealed record GoogleSheetsSubscriptionRow(
        string FullName,
        string DiscordNickname,
        string? Email,
        string Position,
        DateTime? MemberSince,
        DateTime? ExpirationDate,
        DateTime? LastPaymentDate);
}
