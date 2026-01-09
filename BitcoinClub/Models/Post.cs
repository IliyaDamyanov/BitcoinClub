using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BitcoinClub.Models
{
    public sealed class Post
    {
        public Guid Id { get; set; }

        public string AdminUserId { get; set; } = string.Empty;

        public IdentityUser? AdminUser { get; set; }

        public string TextContent { get; set; } = string.Empty;

        public List<string> ImagePaths { get; set; } = new();

        public List<string> Platforms { get; set; } = new();

        public DateTime CreatedAt { get; set; }
    }
}
