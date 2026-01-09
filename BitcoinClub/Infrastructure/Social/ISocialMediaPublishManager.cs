using System.Threading;
using System.Threading.Tasks;
using BitcoinClub.Models;

namespace BitcoinClub.Infrastructure.Social
{
    public interface ISocialMediaPublishManager
    {
        Task PublishAsync(Post post, CancellationToken cancellationToken = default);
    }
}
