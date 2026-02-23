using Microsoft.EntityFrameworkCore;
using MeBeCare.Models;

namespace MeBeCare.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Family> Families { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<PregnancyRecord> PregnancyRecords { get; set; }
        public DbSet<VaccinationRecord> VaccinationRecords { get; set; }
        public DbSet<GrowthRecord> GrowthRecords { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<CommunityPost> CommunityPosts { get; set; } 
        public DbSet<CommunityComment> CommunityComments { get; set; }
        public DbSet<MedicalService> MedicalServices { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Expert> Experts { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Cấu hình cho bảng Users
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserID);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Family)
                .WithOne(f => f.PrimaryUser)
                .HasForeignKey<Family>(f => f.PrimaryUserID)
                .IsRequired(false);

            modelBuilder.Entity<User>()
                .HasOne(u => u.DoctorProfile)
                .WithOne(d => d.User)
                .HasForeignKey<Doctor>(d => d.UserID)
                .IsRequired(false);

            modelBuilder.Entity<User>()
                .HasOne(u => u.ExpertProfile)
                .WithOne(e => e.User)
                .HasForeignKey<Expert>(e => e.UserID)
                .IsRequired(false);

            // 2. Cấu hình cho bảng Families
            modelBuilder.Entity<Family>()
                .HasKey(f => f.FamilyID);

            modelBuilder.Entity<Family>()
                .HasOne(f => f.PrimaryUser)
                .WithOne(u => u.Family)
                .HasForeignKey<Family>(f => f.PrimaryUserID)
                .IsRequired(false); // Sửa thành false để đồng bộ với User

            // 3. Cấu hình cho bảng Children
            modelBuilder.Entity<Child>()
                .HasKey(c => c.ChildID);

            modelBuilder.Entity<Child>()
                .HasOne(c => c.Family)
                .WithMany(f => f.Children)
                .HasForeignKey(c => c.FamilyID)
                .IsRequired(true);

            // 4. Cấu hình cho bảng PregnancyRecords
            modelBuilder.Entity<PregnancyRecord>()
                .HasKey(pr => pr.PregnancyRecordID);

            modelBuilder.Entity<PregnancyRecord>()
                .HasOne(pr => pr.User)
                .WithMany(u => u.PregnancyRecords)
                .HasForeignKey(pr => pr.UserID)
                .IsRequired(true);

            // 5. Cấu hình cho bảng VaccinationRecords
            modelBuilder.Entity<VaccinationRecord>()
                .HasKey(vr => vr.VaccinationID);

            modelBuilder.Entity<VaccinationRecord>()
                .HasOne(vr => vr.Child)
                .WithMany(c => c.VaccinationRecords)
                .HasForeignKey(vr => vr.ChildID)
                .IsRequired(true);

            // 6. Cấu hình cho bảng GrowthRecords
            modelBuilder.Entity<GrowthRecord>()
                .HasKey(gr => gr.GrowthRecordID);

            modelBuilder.Entity<GrowthRecord>()
                .HasOne(gr => gr.Child)
                .WithMany(c => c.GrowthRecords)
                .HasForeignKey(gr => gr.ChildID)
                .IsRequired(true);

            // 7. Cấu hình cho bảng MedicalRecords
            modelBuilder.Entity<MedicalRecord>()
                .HasKey(mr => mr.MedicalRecordID);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.User)
                .WithMany(u => u.MedicalRecords)
                .HasForeignKey(mr => mr.UserID)
                .IsRequired(false);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.Child)
                .WithMany(c => c.MedicalRecords)
                .HasForeignKey(mr => mr.ChildID)
                .IsRequired(false);

            // 8. Cấu hình cho bảng Doctors
            modelBuilder.Entity<Doctor>()
                .HasKey(d => d.DoctorID);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne(u => u.DoctorProfile)
                .HasForeignKey<Doctor>(d => d.UserID)
                .IsRequired(true);

            // 9. Cấu hình cho bảng Appointments
            modelBuilder.Entity<Appointment>()
                .HasKey(a => a.AppointmentID);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany(u => u.AppointmentsAsUser)
                .HasForeignKey(a => a.UserID)
                .IsRequired(true);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorID)
                .IsRequired(true);

            // 10. Cấu hình cho bảng CommunityPosts
            modelBuilder.Entity<CommunityPost>()
                .HasKey(cp => cp.PostID);

            modelBuilder.Entity<CommunityPost>()
                .HasOne(cp => cp.User)
                .WithMany(u => u.CommunityPosts)
                .HasForeignKey(cp => cp.UserID)
                .IsRequired(true);

            // 11. Cấu hình cho bảng CommunityComments
            modelBuilder.Entity<CommunityComment>()
                .HasKey(cc => cc.CommentID);

            modelBuilder.Entity<CommunityComment>()
                .HasOne(cc => cc.User)
                .WithMany(u => u.CommunityComments)
                .HasForeignKey(cc => cc.UserID)
                .IsRequired(true);

            modelBuilder.Entity<CommunityComment>()
                .HasOne(cc => cc.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(cc => cc.PostID)
                .IsRequired(true);

            // 12. Cấu hình cho bảng MedicalServices
            modelBuilder.Entity<MedicalService>()
                .HasKey(ms => ms.ServiceID);

            // 13. Cấu hình cho bảng Expenses
            modelBuilder.Entity<Expense>()
                .HasKey(e => e.ExpenseID);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.User)
                .WithMany(u => u.Expenses)
                .HasForeignKey(e => e.UserID)
                .IsRequired(true);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Child)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.ChildID)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);


            // 14. Cấu hình cho bảng Articles
            modelBuilder.Entity<Article>()
                .HasKey(a => a.ArticleID);

            modelBuilder.Entity<Article>()
                .HasOne(a => a.Author)
                .WithMany(u => u.Articles)
                .HasForeignKey(a => a.AuthorID)
                .IsRequired(true);

            // 15. Cấu hình cho bảng ActivityLogs
            modelBuilder.Entity<ActivityLog>()
                .HasKey(al => al.LogID);

            modelBuilder.Entity<ActivityLog>()
                .HasOne(al => al.User)
                .WithMany(u => u.ActivityLogs)
                .HasForeignKey(al => al.UserID)
                .IsRequired(true);

            // 16. Cấu hình cho bảng Notifications
            modelBuilder.Entity<Notification>()
                .HasKey(n => n.NotificationID);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserID)
                .IsRequired(true);

            // 17. Cấu hình cho bảng Expert
            modelBuilder.Entity<Expert>()
                .HasKey(e => e.ExpertID);

            modelBuilder.Entity<Expert>()
                .HasOne(e => e.User)
                .WithOne(u => u.ExpertProfile)
                .HasForeignKey<Expert>(e => e.UserID)
                .IsRequired();

            // Cấu hình cho bảng ContactMessages
            modelBuilder.Entity<ContactMessage>(entity =>
            {
                entity.HasKey(cm => cm.ContactMessageID);

                entity.Property(cm => cm.SentAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(cm => cm.RepliedAt)
                      .IsRequired(false);

                entity.Property(cm => cm.IsRead)
                      .HasDefaultValue(false);

                entity.Property(cm => cm.IsReplied)
                      .HasDefaultValue(false);

                entity.Property(cm => cm.Reply)
                      .IsRequired(false);

                entity.Property(cm => cm.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(cm => cm.UpdatedAt)
                      .HasDefaultValueSql("GETDATE()");

                // Thiết lập quan hệ với User (người gửi)
                entity.HasOne(cm => cm.User)
                      .WithMany(u => u.ContactMessages)
                      .HasForeignKey(cm => cm.UserID)
                      .IsRequired(true);

                // Thiết lập quan hệ với Admin (người phản hồi)
                entity.HasOne(cm => cm.Admin)
                      .WithMany(u => u.RepliedMessages)
                      .HasForeignKey(cm => cm.AdminID)
                      .IsRequired(false);
            });
        }
    }
}