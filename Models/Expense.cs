using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MeBeCare.Models
{
    public class Expense
    {
        public int ExpenseID { get; set; }
        public int UserID { get; set; }
        public int? ChildID { get; set; }
        public string? Category { get; set; } // Medical, Vaccination, Consultation, Other
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public User? User { get; set; } // Quan hệ nhiều-1 với Users
        public Child? Child { get; set; } // Quan hệ nhiều-1 với Children (có thể null)
    }
}
