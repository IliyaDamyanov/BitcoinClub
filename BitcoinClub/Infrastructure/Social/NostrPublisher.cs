using System.Threading;
using System.Threading.Tasks;
using BitcoinClub.Models;

namespace BitcoinClub.Infrastructure.Social
{
    public sealed class NostrPublisher : ISocialMediaPublisher
    {
        public string Platform => "nostr";

        public Task<PublishResult> PublishAsync(Post post, CancellationToken cancellationToken = default)
            => Task.FromResult(new PublishResult(false, null, "Not implemented."));
    }
}
