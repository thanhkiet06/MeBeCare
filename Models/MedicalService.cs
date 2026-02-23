using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MeBeCare.Models
{
    public class MedicalService
    {
        public int ServiceID { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public decimal? Rating { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
