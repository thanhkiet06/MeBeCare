using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MeBeCare.Models
{
    public class ActivityLog
    {
        public int LogID { get; set; } // Khóa chính, phải có tên là LogID hoặc Id
        public int UserID { get; set; }
        public string? ActivityType { get; set; }
        public string? ActivityData { get; set; }
        public DateTime CreatedAt { get; set; }

        public User? User { get; set; }
    }
}
