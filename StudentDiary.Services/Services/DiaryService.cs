using Microsoft.EntityFrameworkCore;
using StudentDiary.Infrastructure.Data;
using StudentDiary.Infrastructure.Models;
using StudentDiary.Services.DTOs;
using StudentDiary.Services.Interfaces;

namespace StudentDiary.Services.Services
{
    public class DiaryService : IDiaryService
    {
        private readonly StudentDiaryContext _context;

        public DiaryService(StudentDiaryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DiaryEntryDto>> GetUserEntriesAsync(int userId)
        {
            return await _context.DiaryEntries
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.CreatedDate)
                .Select(e => new DiaryEntryDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Content = e.Content,
                    CreatedDate = e.CreatedDate,
                    LastModifiedDate = e.LastModifiedDate,
                    UserId = e.UserId
                })
                .ToListAsync();
        }

        public async Task<DiaryEntryDto> GetEntryByIdAsync(int entryId, int userId)
        {
            var entry = await _context.DiaryEntries.FirstOrDefaultAsync(e => e.Id == entryId && e.UserId == userId);
            if (entry == null) return null;

            return new DiaryEntryDto
            {
                Id = entry.Id,
                Title = entry.Title,
                Content = entry.Content,
                CreatedDate = entry.CreatedDate,
                LastModifiedDate = entry.LastModifiedDate,
                UserId = entry.UserId
            };
        }

        public async Task<(bool Success, string Message, DiaryEntryDto Entry)> CreateEntryAsync(int userId, CreateDiaryEntryDto createDto)
        {
            var entry = new DiaryEntry
            {
                Title = createDto.Title,
                Content = createDto.Content,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                UserId = userId
            };

            _context.DiaryEntries.Add(entry);
            await _context.SaveChangesAsync();

            var dto = new DiaryEntryDto
            {
                Id = entry.Id,
                Title = entry.Title,
                Content = entry.Content,
                CreatedDate = entry.CreatedDate,
                LastModifiedDate = entry.LastModifiedDate,
                UserId = entry.UserId
            };

            return (true, "Entry created.", dto);
        }

        public async Task<(bool Success, string Message, DiaryEntryDto Entry)> UpdateEntryAsync(int userId, UpdateDiaryEntryDto updateDto)
        {
            var entry = await _context.DiaryEntries.FirstOrDefaultAsync(e => e.Id == updateDto.Id && e.UserId == userId);
            if (entry == null)
            {
                return (false, "Entry not found or access denied.", null);
            }

            entry.Title = updateDto.Title;
            entry.Content = updateDto.Content;
            entry.LastModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var dto = new DiaryEntryDto
            {
                Id = entry.Id,
                Title = entry.Title,
                Content = entry.Content,
                CreatedDate = entry.CreatedDate,
                LastModifiedDate = entry.LastModifiedDate,
                UserId = entry.UserId
            };

            return (true, "Entry updated.", dto);
        }

        public async Task<(bool Success, string Message)> DeleteEntryAsync(int entryId, int userId)
        {
            var entry = await _context.DiaryEntries.FirstOrDefaultAsync(e => e.Id == entryId && e.UserId == userId);
            if (entry == null)
            {
                return (false, "Entry not found or access denied.");
            }

            _context.DiaryEntries.Remove(entry);
            await _context.SaveChangesAsync();
            return (true, "Entry deleted.");
        }
    }
}

