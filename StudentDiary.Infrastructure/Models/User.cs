using System.ComponentModel.DataAnnotations;

namespace StudentDiary.Infrastructure.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Email { get; set; }
        
        [Required]
        public string PasswordHash { get; set; }
        
        [StringLength(50)]
        public string FirstName { get; set; }
        
        [StringLength(50)]
        public string LastName { get; set; }
        
        public string ProfilePicturePath { get; set; }
        
        public DateTime DateCreated { get; set; }
        
        public DateTime LastLoginDate { get; set; }
        
        public int FailedLoginAttempts { get; set; }
        
        public DateTime? LockoutEnd { get; set; }
        
        public string PasswordResetToken { get; set; }
        
        public DateTime? PasswordResetTokenExpiry { get; set; }
        
        // Navigation property
        public virtual ICollection<DiaryEntry> DiaryEntries { get; set; } = new List<DiaryEntry>();
    }
}