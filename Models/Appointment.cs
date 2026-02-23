using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MeBeCare.Models
{
    public class Appointment
    {
        public int AppointmentID { get; set; }
        public int UserID { get; set; }
        public int DoctorID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? Status { get; set; } // Pending, Confirmed, Completed, Cancelled
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public User? User { get; set; } // Quan hệ nhiều-1 với Users (người đặt lịch)
        public Doctor? Doctor { get; set; } // Quan hệ nhiều-1 với Doctors
    }
}
