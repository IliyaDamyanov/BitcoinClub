using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BitcoinClub.Infrastructure.Files
{
    public interface IFileUploadService
    {
        Task<IReadOnlyList<string>> SavePostImagesAsync(
            IEnumerable<IFormFile> files,
            CancellationToken cancellationToken = default);
    }
}
