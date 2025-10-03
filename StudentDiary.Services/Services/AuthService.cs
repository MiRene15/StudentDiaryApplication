using Microsoft.EntityFrameworkCore;
using StudentDiary.Infrastructure.Data;
using StudentDiary.Infrastructure.Models;
using StudentDiary.Services.DTOs;
using StudentDiary.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace StudentDiary.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly StudentDiaryContext _context;

        public AuthService(StudentDiaryContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto registerDto)
        {
            // Basic validation beyond DataAnnotations
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                return (false, "Passwords do not match.");
            }

            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email);

            if (existingUser != null)
            {
                return (false, "Username or email already exists.");
            }

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                FirstName = registerDto.FirstName ?? string.Empty,
                LastName = registerDto.LastName ?? string.Empty,
                ProfilePicturePath = string.Empty,
                DateCreated = DateTime.UtcNow,
                LastLoginDate = DateTime.UtcNow,
                FailedLoginAttempts = 0,
                LockoutEnd = null,
                PasswordResetToken = string.Empty,
                PasswordResetTokenExpiry = null
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return (true, "User registered successfully.");
        }

        public async Task<(bool Success, string Message, UserProfileDto User)> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
            if (user == null)
            {
                return (false, "Invalid username or password.", null);
            }

            // Lockout check
            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
            {
                return (false, $"Account locked until {user.LockoutEnd.Value:u}.", null);
            }

            if (!VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                user.FailedLoginAttempts += 1;
                if (user.FailedLoginAttempts >= 3)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                    user.FailedLoginAttempts = 0; // reset counter after lockout
                }
                await _context.SaveChangesAsync();
                return (false, "Invalid username or password.", null);
            }

            // Successful login
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var profile = new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePicturePath = user.ProfilePicturePath,
                DateCreated = user.DateCreated
            };

            return (true, "Login successful.", profile);
        }

        public async Task<(bool Success, string Message)> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == forgotPasswordDto.Email);
            if (user == null)
            {
                // Return true to avoid leaking which emails are registered
                return (true, "If an account with that email exists, a reset link has been sent.");
            }

            user.PasswordResetToken = GenerateSecureToken();
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _context.SaveChangesAsync();

            // In a real app, you would send an email with the token link.
            return (true, "Password reset token generated.");
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == resetPasswordDto.Token);
            if (user == null)
            {
                return (false, "Invalid or expired token.");
            }

            if (!user.PasswordResetTokenExpiry.HasValue || user.PasswordResetTokenExpiry.Value < DateTime.UtcNow)
            {
                return (false, "Token has expired.");
            }

            user.PasswordHash = HashPassword(resetPasswordDto.NewPassword);
            user.PasswordResetToken = string.Empty;
            user.PasswordResetTokenExpiry = null;
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            await _context.SaveChangesAsync();

            return (true, "Password has been reset.");
        }

        public async Task<UserProfileDto> GetUserProfileAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            return new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePicturePath = user.ProfilePicturePath,
                DateCreated = user.DateCreated
            };
        }

        public async Task<(bool Success, string Message)> UpdateProfileAsync(int userId, UpdateProfileDto updateProfileDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return (false, "User not found.");

            if (!string.IsNullOrWhiteSpace(updateProfileDto.FirstName))
                user.FirstName = updateProfileDto.FirstName;
            if (!string.IsNullOrWhiteSpace(updateProfileDto.LastName))
                user.LastName = updateProfileDto.LastName;
            if (!string.IsNullOrWhiteSpace(updateProfileDto.Email))
            {
                // Ensure new email is unique
                var exists = await _context.Users.AnyAsync(u => u.Email == updateProfileDto.Email && u.Id != userId);
                if (exists)
                {
                    return (false, "Email is already in use.");
                }
                user.Email = updateProfileDto.Email;
            }

            await _context.SaveChangesAsync();
            return (true, "Profile updated.");
        }

        public async Task<(bool Success, string Message)> UpdateProfilePictureAsync(int userId, string imagePath)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return (false, "User not found.");

            user.ProfilePicturePath = imagePath ?? string.Empty;
            await _context.SaveChangesAsync();
            return (true, "Profile picture updated.");
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        public bool VerifyPassword(string password, string hash)
        {
            var computed = HashPassword(password);
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(computed),
                Encoding.UTF8.GetBytes(hash));
        }

        private static string GenerateSecureToken()
        {
            var bytes = new byte[32];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}

