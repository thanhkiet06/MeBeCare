using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MeBeCare.Models
{
    public class CommunityComment
    {
        public int CommentID { get; set; } // Khóa chính
        public int PostID { get; set; }
        public int UserID { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public CommunityPost? Post { get; set; }
        public User? User { get; set; }
    }
}