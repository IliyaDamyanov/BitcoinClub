using System;

namespace BitcoinClub.Areas.Admin.ViewModels
{
    public sealed class PostListItemViewModel
    {
        public Guid Id { get; set; }

        public string TextContent { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public string PlatformsCsv { get; set; } = string.Empty;
    }
}
