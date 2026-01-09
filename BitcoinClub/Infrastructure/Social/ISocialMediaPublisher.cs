using System.Threading;
using System.Threading.Tasks;
using BitcoinClub.Models;

namespace BitcoinClub.Infrastructure.Social
{
    public interface ISocialMediaPublisher
    {
        string Platform { get; }

        Task<PublishResult> PublishAsync(Post post, CancellationToken cancellationToken = default);
    }
}
