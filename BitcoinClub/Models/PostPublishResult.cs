using System;

namespace BitcoinClub.Models
{
    public sealed class PostPublishResult
    {
        public Guid Id { get; set; }

        public Guid PostId { get; set; }

        public Post? Post { get; set; }

        public string Platform { get; set; } = string.Empty;

        public bool Success { get; set; }

        public string? ProviderPostId { get; set; }

        public string? Error { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
