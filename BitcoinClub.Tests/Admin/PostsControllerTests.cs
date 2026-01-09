using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BitcoinClub.Areas.Admin.Controllers;
using BitcoinClub.Areas.Admin.ViewModels;
using BitcoinClub.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BitcoinClub.Tests.Admin
{
    public class PostsControllerTests
    {
        [Fact]
        public async Task Create_Post_WithValidModel_SavesPost()
        {
            var db = CreateInMemoryDb();
            db.Users.Add(new Microsoft.AspNetCore.Identity.IdentityUser { Id = "admin-1", UserName = "a@test.local", Email = "a@test.local" });
            await db.SaveChangesAsync();

            var uploads = new FakeUploads();

            var controller = new PostsController(db, uploads);
            controller.ControllerContext = new ControllerContext { HttpContext = CreateHttpContext("admin-1") };

            var vm = new PostCreateViewModel
            {
                TextContent = "hello",
                SelectedPlatforms = { "nostr" }
            };

            var result = await controller.Create(vm);

            Assert.IsType<RedirectToActionResult>(result);

            var post = await db.Posts.SingleAsync();
            Assert.Equal("admin-1", post.AdminUserId);
            Assert.Equal("hello", post.TextContent);
            Assert.Contains("nostr", post.Platforms);
        }

        private static ApplicationDbContext CreateInMemoryDb()
        {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .Options;

            return new ApplicationDbContext(opts);
        }

        private static DefaultHttpContext CreateHttpContext(string userId)
        {
            var ctx = new DefaultHttpContext();
            ctx.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "Test"));
            return ctx;
        }

        private sealed class FakeUploads : BitcoinClub.Infrastructure.Files.IFileUploadService
        {
            public Task<System.Collections.Generic.IReadOnlyList<string>> SavePostImagesAsync(System.Collections.Generic.IEnumerable<IFormFile> files, CancellationToken cancellationToken = default)
                => Task.FromResult<System.Collections.Generic.IReadOnlyList<string>>(new[] { "uploads/posts/a.png" });
        }
    }
}
