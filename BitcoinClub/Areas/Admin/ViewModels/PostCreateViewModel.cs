using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BitcoinClub.Areas.Admin.ViewModels
{
    public sealed class PostCreateViewModel
    {
        [Required]
        public string TextContent { get; set; } = string.Empty;

        public List<IFormFile> Images { get; set; } = new();

        public List<string> SelectedPlatforms { get; set; } = new();

        public static readonly string[] SupportedPlatforms = new[] { "facebook", "instagram", "threads", "twitter", "nostr" };
    }
}
