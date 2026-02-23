using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MeBeCare.Models
{
    public class GrowthRecord
    {
        public int GrowthRecordID { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn bé")]
        public int ChildID { get; set; }

        public DateTime RecordDate { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public decimal? HeadCircumference { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Child? Child { get; set; } // Quan hệ nhiều-1 với Children

    }
}
