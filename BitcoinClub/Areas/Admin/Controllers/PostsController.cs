using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BitcoinClub.Areas.Admin.ViewModels;
using BitcoinClub.Data;
using BitcoinClub.Infrastructure.Auth;
using BitcoinClub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BitcoinClub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleNames.Admin)]
    public sealed class PostsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public PostsController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
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

            var uploadRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "posts");
            Directory.CreateDirectory(uploadRoot);

            var imagePaths = model.Images
                .Where(f => f?.Length > 0)
                .Select(async f =>
                {
                    var ext = Path.GetExtension(f.FileName);
                    var name = $"{Guid.NewGuid():N}{ext}";
                    var absPath = Path.Combine(uploadRoot, name);
                    await using var stream = System.IO.File.Create(absPath);
                    await f.CopyToAsync(stream);
                    return $"/uploads/posts/{name}";
                })
                .ToArray();

            var savedPaths = await Task.WhenAll(imagePaths);

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
