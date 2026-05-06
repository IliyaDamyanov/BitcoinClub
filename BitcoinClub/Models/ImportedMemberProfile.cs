using System;
using Microsoft.AspNetCore.Identity;

namespace BitcoinClub.Models
{
    public sealed class ImportedMemberProfile
    {
        public Guid Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public IdentityUser? User { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string DiscordNickname { get; set; } = string.Empty;

        public string Position { get; set; } = string.Empty;

        public DateTime? MemberSince { get; set; }

        public string TotalContributionsRaw { get; set; } = string.Empty;

        public string VolunteerInterests { get; set; } = string.Empty;

        public string StreetAddress { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        public string PostalCode { get; set; } = string.Empty;

        public string SecondaryEmail { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public DateTime UpdatedAt { get; set; }
    }
}
