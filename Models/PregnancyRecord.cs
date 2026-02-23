using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Numerics;

namespace MeBeCare.Models
{
    public class PregnancyRecord
    {
        public int PregnancyRecordID { get; set; }
        public int UserID { get; set; }
        public int Week { get; set; }
        public DateTime StartDate { get; set; }
        public decimal? Weight { get; set; }
        public string? BloodPressure { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // Navigation properties
        public User? User { get; set; } // Quan hệ nhiều-1 với Users
    }
}
