using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using StudentDiary.Presentation.Filters;
using StudentDiary.Services.Interfaces;
using StudentDiary.Services.DTOs;

namespace StudentDiary.Presentation.Controllers
{
    [SessionAuthorize]
    public class ProfileController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(IAuthService authService, IWebHostEnvironment env, ILogger<ProfileController> logger)
        {
            _authService = authService;
            _env = env;
            _logger = logger;
        }

        private int CurrentUserId => HttpContext.Session.GetInt32("UserId")!.Value;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var profile = await _authService.GetUserProfileAsync(CurrentUserId);
            if (profile == null) return NotFound();
            return View(profile);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var profile = await _authService.GetUserProfileAsync(CurrentUserId);
            if (profile == null) return NotFound();
            var model = new UpdateProfileDto
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Email = profile.Email
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateProfileDto model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _authService.UpdateProfileAsync(CurrentUserId, model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }
            TempData["StatusMessage"] = "Profile updated.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["StatusMessage"] = "No file selected.";
                return RedirectToAction("Index");
            }

            // Basic validation
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext))
            {
                TempData["StatusMessage"] = "Invalid file type.";
                return RedirectToAction("Index");
            }
            if (file.Length > 5 * 1024 * 1024) // 5 MB
            {
                TempData["StatusMessage"] = "File too large.";
                return RedirectToAction("Index");
            }

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsDir);
            var fileName = $"user_{CurrentUserId}_{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploadsDir, fileName);
            using (var stream = System.IO.File.Create(fullPath))
            {
                await file.CopyToAsync(stream);
            }

            var relative = $"/uploads/{fileName}";
            await _authService.UpdateProfilePictureAsync(CurrentUserId, relative);
            TempData["StatusMessage"] = "Profile picture updated.";
            return RedirectToAction("Index");
        }
    }
}

