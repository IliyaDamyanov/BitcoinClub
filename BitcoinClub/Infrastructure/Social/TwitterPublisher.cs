using System.Threading;
using System.Threading.Tasks;
using BitcoinClub.Models;

namespace BitcoinClub.Infrastructure.Social
{
    public sealed class TwitterPublisher : ISocialMediaPublisher
    {
        public string Platform => "twitter";

        public Task<PublishResult> PublishAsync(Post post, CancellationToken cancellationToken = default)
            => Task.FromResult(new PublishResult(false, null, "Not implemented."));
    }
}
