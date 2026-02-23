using System;
using System.ComponentModel.DataAnnotations;

namespace MeBeCare.Models
{
    public class Expert
    {
        public int ExpertID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn người dùng")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lĩnh vực chuyên môn")]
        [StringLength(100)]
        public string? Field { get; set; }

        [StringLength(100)]
        public string? Degree { get; set; }

        [Range(0, 100, ErrorMessage = "Số năm kinh nghiệm không hợp lệ")]
        public int? ExperienceYears { get; set; }

        [StringLength(1000)]
        public string? Bio { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string? ProfilePicture { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation
        public User? User { get; set; } // Quan hệ nhiều-1 với Users
    }
}
