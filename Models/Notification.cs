using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MeBeCare.Models
{
    public class Notification
    {
        public int NotificationID { get; set; }
        public int UserID { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User? User { get; set; } // Quan hệ nhiều-1 với Users
    }
}
