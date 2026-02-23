using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MeBeCare.Models
{
    public class Article
    {
        public int ArticleID { get; set; }

        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        public string? Title { get; set; }

        [Display(Name = "Nội dung")]
        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string? Content { get; set; }

        public int AuthorID { get; set; } // Lưu UserID khi đăng nhập

        public string Category { get; set; } = "ChuyenGia";

        public string? ImageUrl { get; set; } // KHÔNG bắt buộc
        public string? VideoUrl { get; set; } // KHÔNG bắt buộc

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public User? Author { get; set; } // KHÔNG đánh dấu [Required]
    }

}
