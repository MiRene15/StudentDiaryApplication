using System.ComponentModel.DataAnnotations;

namespace StudentDiary.Infrastructure.Models
{
    public class DiaryEntry
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [Required]
        public string Content { get; set; } // Rich text content
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime LastModifiedDate { get; set; }
        
        // Foreign key
        public int UserId { get; set; }
        
        // Navigation property
        public virtual User User { get; set; }
    }
}