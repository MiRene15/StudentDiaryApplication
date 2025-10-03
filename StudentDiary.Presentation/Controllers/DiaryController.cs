using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using StudentDiary.Presentation.Filters;
using StudentDiary.Services.DTOs;
using StudentDiary.Services.Interfaces;

namespace StudentDiary.Presentation.Controllers
{
    [SessionAuthorize]
    public class DiaryController : Controller
    {
        private readonly IDiaryService _diaryService;
        private readonly ILogger<DiaryController> _logger;

        public DiaryController(IDiaryService diaryService, ILogger<DiaryController> logger)
        {
            _diaryService = diaryService;
            _logger = logger;
        }

        private int CurrentUserId => HttpContext.Session.GetInt32("UserId")!.Value;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var entries = await _diaryService.GetUserEntriesAsync(CurrentUserId);
            return View(entries);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateDiaryEntryDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDiaryEntryDto model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _diaryService.CreateEntryAsync(CurrentUserId, model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }
            TempData["StatusMessage"] = "Entry created.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var entry = await _diaryService.GetEntryByIdAsync(id, CurrentUserId);
            if (entry == null) return NotFound();
            var model = new UpdateDiaryEntryDto { Id = entry.Id, Title = entry.Title, Content = entry.Content };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateDiaryEntryDto model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _diaryService.UpdateEntryAsync(CurrentUserId, model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }
            TempData["StatusMessage"] = "Entry updated.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _diaryService.DeleteEntryAsync(id, CurrentUserId);
            if (!result.Success)
            {
                TempData["StatusMessage"] = result.Message;
            }
            else
            {
                TempData["StatusMessage"] = "Entry deleted.";
            }
            return RedirectToAction("Index");
        }
    }
}

