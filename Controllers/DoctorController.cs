using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeBeCare.Data;
using MeBeCare.Models;
using System.Linq;
using System.Threading.Tasks;
using MeBeCare.Services;

namespace MeBeCare.Controllers
{
    public class DoctorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public DoctorController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // Danh sách bác sĩ
        public async Task<IActionResult> Index(string searchString)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var doctors = _context.Doctors.Include(d => d.User).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                doctors = doctors.Where(d =>
                    d.User.FirstName.Contains(searchString) ||
                    d.User.LastName.Contains(searchString) ||
                    d.Specialty.Contains(searchString) ||
                    d.Hospital.Contains(searchString));
            }

            ViewBag.SearchString = searchString;
            return View(await doctors.ToListAsync());
        }


        // Xem chi tiết bác sĩ
        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.DoctorID == id);

            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        // GET: Đặt lịch tư vấn
        [HttpGet]
        public async Task<IActionResult> BookAppointment()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Doctors = await _context.Doctors.Include(d => d.User).ToListAsync();
            return View();
        }

        // [POST] Đặt lịch hẹn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment(Appointment appointment)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                ViewBag.Doctors = await _context.Doctors.Include(d => d.User).ToListAsync();
                return View(appointment);
            }

            // Kiểm tra ngày hẹn
            if (appointment.AppointmentDate < DateTime.Now.AddHours(1))
            {
                ModelState.AddModelError("AppointmentDate", "Ngày hẹn phải ít nhất 1 giờ sau thời điểm hiện tại.");
                ViewBag.Doctors = await _context.Doctors.Include(d => d.User).ToListAsync();
                return View(appointment);
            }

            // Kiểm tra bác sĩ bận
            var existingAppointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.DoctorID == appointment.DoctorID &&
                                          a.AppointmentDate == appointment.AppointmentDate);
            if (existingAppointment != null)
            {
                ModelState.AddModelError("AppointmentDate", "Bác sĩ đã có lịch hẹn vào thời gian này.");
                ViewBag.Doctors = await _context.Doctors.Include(d => d.User).ToListAsync();
                return View(appointment);
            }

            // Kiểm tra người dùng bận
            var existingUserAppointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.UserID == userId.Value &&
                                          a.AppointmentDate == appointment.AppointmentDate);
            if (existingUserAppointment != null)
            {
                ModelState.AddModelError("AppointmentDate", "Bạn đã có lịch hẹn vào thời gian này.");
                ViewBag.Doctors = await _context.Doctors.Include(d => d.User).ToListAsync();
                return View(appointment);
            }

            // Lưu lịch hẹn
            appointment.UserID = userId.Value;
            appointment.Status = "Pending";
            appointment.CreatedAt = DateTime.Now;
            appointment.UpdatedAt = DateTime.Now;

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Lấy thông tin bác sĩ và người dùng
            var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(d => d.DoctorID == appointment.DoctorID);
            var user = await _context.Users.FindAsync(userId.Value);

            // Gửi email cho bác sĩ
            if (doctor != null)
            {
                var emailBody = $@"
            <h3>Bạn có một lịch hẹn mới từ hệ thống MeBeCare</h3>
            <p><strong>Thời gian:</strong> {appointment.AppointmentDate:dd/MM/yyyy HH:mm}</p>
            <p><strong>Ghi chú từ bệnh nhân:</strong> {appointment.Notes}</p>
            <p>Vui lòng đăng nhập hệ thống để xác nhận lịch hẹn.</p>";

                await _emailService.SendEmailAsync(
                    doctor.User.Email,
                    "📅 Lịch hẹn mới từ hệ thống MeBeCare",
                    emailBody
                );
            }

            // Gửi email xác nhận cho người dùng
            if (user != null && doctor != null)
            {
                var userEmailBody = $@"
            <h3>Thông báo đặt lịch hẹn thành công</h3>
            <p>Bạn đã đặt lịch hẹn với bác sĩ {doctor.User.FirstName} {doctor.User.LastName}.</p>
            <p><strong>Thời gian:</strong> {appointment.AppointmentDate:dd/MM/yyyy HH:mm}</p>
            <p><strong>Ghi chú:</strong> {appointment.Notes}</p>";

                await _emailService.SendEmailAsync(
                    user.Email,
                    "📅 Đặt lịch hẹn thành công",
                    userEmailBody
                );
            }

            TempData["SuccessMessage"] = "📅 Đã đặt lịch hẹn thành công!";
            return RedirectToAction("Index", "Doctor");
        }

    }
}
