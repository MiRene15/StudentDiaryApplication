using System.ComponentModel.DataAnnotations;

namespace StudentDiary.Services.DTOs
{
    public class CreateDiaryEntryDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [Required]
        public string Content { get; set; }
    }
    
    public class UpdateDiaryEntryDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [Required]
        public string Content { get; set; }
    }
    
    public class DiaryEntryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int UserId { get; set; }
    }
}