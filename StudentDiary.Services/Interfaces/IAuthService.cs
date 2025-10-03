using StudentDiary.Services.DTOs;

namespace StudentDiary.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message)> RegisterAsync(RegisterDto registerDto);
        Task<(bool Success, string Message, UserProfileDto User)> LoginAsync(LoginDto loginDto);
        Task<(bool Success, string Message)> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<UserProfileDto> GetUserProfileAsync(int userId);
        Task<(bool Success, string Message)> UpdateProfileAsync(int userId, UpdateProfileDto updateProfileDto);
        Task<(bool Success, string Message)> UpdateProfilePictureAsync(int userId, string imagePath);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}