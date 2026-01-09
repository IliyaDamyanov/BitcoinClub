using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BitcoinClub.Data;
using BitcoinClub.Models;
using Microsoft.EntityFrameworkCore;

namespace BitcoinClub.Infrastructure.Social
{
    public sealed class SocialMediaPublishManager : ISocialMediaPublishManager
    {
        private readonly ApplicationDbContext _db;
        private readonly IReadOnlyList<ISocialMediaPublisher> _publishers;

        public SocialMediaPublishManager(ApplicationDbContext db, IEnumerable<ISocialMediaPublisher> publishers)
        {
            _db = db;
            _publishers = publishers.ToList();
        }

        public async Task PublishAsync(Post post, CancellationToken cancellationToken = default)
        {
            if (post is null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            if (post.Id == Guid.Empty)
            {
                throw new ArgumentException("Post.Id is required.", nameof(post));
            }

            var platformKeys = (post.Platforms ?? new List<string>())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            if (platformKeys.Length == 0)
            {
                return;
            }

            // Ensure the Post exists in the DB for FK integrity.
            var exists = await _db.Posts.AnyAsync(p => p.Id == post.Id, cancellationToken);
            if (!exists)
            {
                _db.Posts.Add(post);
                await _db.SaveChangesAsync(cancellationToken);
            }

            foreach (var platform in platformKeys)
            {
                var publisher = _publishers.FirstOrDefault(p => string.Equals(p.Platform, platform, StringComparison.Ordinal));

                PublishResult res;
                if (publisher is null)
                {
                    res = new PublishResult(false, null, "No publisher registered for platform.");
                }
                else
                {
                    res = await publisher.PublishAsync(post, cancellationToken);
                }

                // Idempotency: only one result row per (PostId, Platform)
                var row = await _db.PostPublishResults
                    .SingleOrDefaultAsync(r => r.PostId == post.Id && r.Platform == platform, cancellationToken);

                if (row is null)
                {
                    row = new PostPublishResult
                    {
                        Id = Guid.NewGuid(),
                        PostId = post.Id,
                        Platform = platform,
                        CreatedAt = DateTime.UtcNow
                    };

                    _db.PostPublishResults.Add(row);
                }

                row.Success = res.Success;
                row.ProviderPostId = res.ProviderPostId;
                row.Error = res.Error;

                await _db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
