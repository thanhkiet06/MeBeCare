using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MeBeCare.Models
{
    public class Child
    {
        public int ChildID { get; set; }
        public int FamilyID { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string? Gender { get; set; } // Male, Female, Other

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ValidateNever]
        public Family? Family { get; set; }

        [ValidateNever]
        public ICollection<VaccinationRecord>? VaccinationRecords { get; set; }

        [ValidateNever]
        public ICollection<GrowthRecord>? GrowthRecords { get; set; }

        [ValidateNever]
        public ICollection<MedicalRecord>? MedicalRecords { get; set; }

        [ValidateNever]
        public ICollection<Expense>? Expenses { get; set; }
    }
}
