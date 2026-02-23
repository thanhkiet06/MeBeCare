using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MeBeCare.Models
{
    public class Doctor
    {
        public int DoctorID { get; set; }
        public int UserID { get; set; }
        public string? Specialty { get; set; }
        public string? Hospital { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User? User { get; set; } // Quan hệ nhiều-1 với Users
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
