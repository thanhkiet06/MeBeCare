using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MeBeCare.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? PasswordHash { get; set; }
        public string? Role { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties - Đánh dấu là nullable
        public Family? Family { get; set; } // Quan hệ 1-1 với Families (PrimaryUser)
        public Doctor? DoctorProfile { get; set; } // Quan hệ 1-1 với Doctors (nếu là Doctor)
        public ICollection<PregnancyRecord>? PregnancyRecords { get; set; } // Quan hệ 1-nhiều với PregnancyRecords
        public ICollection<MedicalRecord>? MedicalRecords { get; set; } // Quan hệ 1-nhiều với MedicalRecords
        public ICollection<Appointment>? AppointmentsAsUser { get; set; } // Quan hệ 1-nhiều với Appointments (người đặt lịch)
        public ICollection<CommunityPost>? CommunityPosts { get; set; } // Quan hệ 1-nhiều với CommunityPosts
        public ICollection<CommunityComment>? CommunityComments { get; set; } // Quan hệ 1-nhiều với CommunityComments
        public ICollection<Expense>? Expenses { get; set; } // Quan hệ 1-nhiều với Expenses
        public ICollection<Article>? Articles { get; set; } // Quan hệ 1-nhiều với Articles
        public ICollection<ActivityLog>? ActivityLogs { get; set; } // Quan hệ 1-nhiều với ActivityLogs
        public ICollection<Notification>? Notifications { get; set; } // Quan hệ 1-nhiều với Notifications
        public List<ContactMessage> ContactMessages { get; set; } = new List<ContactMessage>();
        public List<ContactMessage> RepliedMessages { get; set; } = new List<ContactMessage>();
        public Expert? ExpertProfile { get; set; } // hoặc bỏ hoàn toàn [Required]


    }
}
