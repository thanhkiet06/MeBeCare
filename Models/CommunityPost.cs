using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MeBeCare.Models
{
    public class CommunityPost
    {
        public int PostID { get; set; }
        public int UserID { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User? User { get; set; }
        public ICollection<CommunityComment>? Comments { get; set; }
    }
}
