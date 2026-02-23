using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MeBeCare.Models
{
    public class VaccinationRecord
    {
        public int VaccinationID { get; set; }
        [Required]
        public int ChildID { get; set; }
        [Required]
        public string? VaccineName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime VaccinationDate { get; set; }
        [Required]
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Child? Child { get; set; } // Quan hệ nhiều-1 với Children
    }
}
