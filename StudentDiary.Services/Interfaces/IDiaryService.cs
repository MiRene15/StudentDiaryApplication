using StudentDiary.Services.DTOs;

namespace StudentDiary.Services.Interfaces
{
    public interface IDiaryService
    {
        Task<IEnumerable<DiaryEntryDto>> GetUserEntriesAsync(int userId);
        Task<DiaryEntryDto> GetEntryByIdAsync(int entryId, int userId);
        Task<(bool Success, string Message, DiaryEntryDto Entry)> CreateEntryAsync(int userId, CreateDiaryEntryDto createDto);
        Task<(bool Success, string Message, DiaryEntryDto Entry)> UpdateEntryAsync(int userId, UpdateDiaryEntryDto updateDto);
        Task<(bool Success, string Message)> DeleteEntryAsync(int entryId, int userId);
    }
}