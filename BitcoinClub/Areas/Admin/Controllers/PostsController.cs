using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BitcoinClub.Areas.Admin.ViewModels;
using BitcoinClub.Data;
using BitcoinClub.Infrastructure.Auth;
using BitcoinClub.Infrastructure.Files;
using BitcoinClub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BitcoinClub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleNames.Admin)]
    public sealed class PostsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IFileUploadService _uploads;

        public PostsController(ApplicationDbContext db, IFileUploadService uploads)
        {
            _db = db;
            _uploads = uploads;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var items = await _db.Posts
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PostListItemViewModel
                {
                    Id = p.Id,
                    TextContent = p.TextContent,
                    CreatedAt = p.CreatedAt,
                    PlatformsCsv = string.Join(", ", p.Platforms)
                })
                .ToListAsync();

            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new PostCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostCreateViewModel model)
        {
            var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(adminUserId))
            {
                return Challenge();
            }

            if (model.SelectedPlatforms.Count == 0)
            {
                ModelState.AddModelError(nameof(model.SelectedPlatforms), "Select at least one platform.");
            }

            var invalidPlatform = model.SelectedPlatforms.FirstOrDefault(p => !PostCreateViewModel.SupportedPlatforms.Contains(p));
            if (invalidPlatform is not null)
            {
                ModelState.AddModelError(nameof(model.SelectedPlatforms), "Invalid platform.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var savedPaths = await _uploads.SavePostImagesAsync(model.Images);

            var post = new Post
            {
                Id = Guid.NewGuid(),
                AdminUserId = adminUserId,
                TextContent = model.TextContent,
                CreatedAt = DateTime.UtcNow,
                ImagePaths = savedPaths.ToList(),
                Platforms = model.SelectedPlatforms.ToList()
            };

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
