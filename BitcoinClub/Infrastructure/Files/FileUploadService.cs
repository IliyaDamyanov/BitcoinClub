using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BitcoinClub.Infrastructure.Files
{
    public sealed class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IUploadPathValidator _validator;

        public FileUploadService(IWebHostEnvironment env, IUploadPathValidator validator)
        {
            _env = env;
            _validator = validator;
        }

        public async Task<IReadOnlyList<string>> SavePostImagesAsync(
            IEnumerable<IFormFile> files,
            CancellationToken cancellationToken = default)
        {
            var webRoot = _env.WebRootPath ?? "wwwroot";

            // Task requirement: save under /wwwroot/uploads
            var uploadRoot = Path.Combine(webRoot, "uploads", "posts");
            Directory.CreateDirectory(uploadRoot);

            var result = new List<string>();

            foreach (var file in files ?? Enumerable.Empty<IFormFile>())
            {
                if (file is null || file.Length <= 0)
                {
                    continue;
                }

                if (!_validator.IsAllowedFileName(file.FileName))
                {
                    continue;
                }

                var ext = Path.GetExtension(file.FileName);
                if (!_validator.IsAllowedExtension(ext))
                {
                    continue;
                }

                var name = $"{Guid.NewGuid():N}{ext.ToLowerInvariant()}";
                var absPath = Path.Combine(uploadRoot, name);

                await using (var stream = File.Create(absPath))
                {
                    await file.CopyToAsync(stream, cancellationToken);
                }

                // Store relative path in DB.
                result.Add(Path.Combine("uploads", "posts", name).Replace('\\', '/'));
            }

            return result;
        }
    }
}
