using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BitcoinClub.Infrastructure.Files;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace BitcoinClub.Tests.Files
{
    public class FileUploadServiceTests
    {
        [Fact]
        public async Task SavePostImagesAsync_SavesAllowedFile_AndReturnsRelativePath()
        {
            var root = CreateTempDir();

            var env = new Mock<IWebHostEnvironment>();
            env.Setup(e => e.WebRootPath).Returns(root);

            var svc = new FileUploadService(env.Object, new UploadPathValidator());

            var file = CreateFormFile("a.png", "hello");
            var paths = await svc.SavePostImagesAsync(new[] { file });

            Assert.Single(paths);
            Assert.StartsWith("uploads/posts/", paths[0]);

            var abs = Path.Combine(root, paths[0].Replace('/', Path.DirectorySeparatorChar));
            Assert.True(File.Exists(abs));
        }

        [Fact]
        public async Task SavePostImagesAsync_RejectsDisallowedExtension()
        {
            var root = CreateTempDir();

            var env = new Mock<IWebHostEnvironment>();
            env.Setup(e => e.WebRootPath).Returns(root);

            var svc = new FileUploadService(env.Object, new UploadPathValidator());

            var file = CreateFormFile("a.gif", "hello");
            var paths = await svc.SavePostImagesAsync(new[] { file });

            Assert.Empty(paths);
        }

        private static IFormFile CreateFormFile(string fileName, string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "Images", fileName);
        }

        private static string CreateTempDir()
        {
            var dir = Path.Combine(Path.GetTempPath(), "btcclub_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
