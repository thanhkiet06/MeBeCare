using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MeBeCare.Models
{
    public class MedicalRecord
    {
        public int MedicalRecordID { get; set; }
        public int? UserID { get; set; }
        public int? ChildID { get; set; }
        [Required(ErrorMessage = "Loại hồ sơ không được bỏ trống")]
        public string? RecordType { get; set; } // Allergy, Condition, Medication, Other
        [Required(ErrorMessage = "Mô tả không được bỏ trống")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Mô tả phải từ 10 đến 500 ký tự")]
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }     // ← nullable
        public virtual Child? Child { get; set; }
    }
}
