using System;
using System.ComponentModel.DataAnnotations;

namespace MeBeCare.Models
{
    public class ContactMessage
    {
        public int ContactMessageID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(200)]
        [Display(Name = "Họ và tên")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(200)]
        public string? Email { get; set; }

        [StringLength(50)]
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [StringLength(200)]
        [Display(Name = "Chủ đề")]
        public string? Subject { get; set; }

        [StringLength(100)]
        [Display(Name = "Độ tuổi của con")]
        public string? ChildAge { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        [Display(Name = "Nội dung tin nhắn")]
        public string? Message { get; set; }

        [Display(Name = "Phản hồi từ admin")]
        public string? AdminReply { get; set; }

        public bool IsRead { get; set; } = false;

        public bool IsReplied { get; set; } = false;

        public string? Reply { get; set; }

        public DateTime SentAt { get; set; } = DateTime.Now;

        public DateTime? RepliedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Required]
        public int UserID { get; set; }

        public int? AdminID { get; set; }

        // 🔗 Navigation properties
        public User? User { get; set; }

        public User? Admin { get; set; }
    }
}
