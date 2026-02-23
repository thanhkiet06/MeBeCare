using MeBeCare.Data;
using MeBeCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MeBeCare.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Trang danh sách người dùng
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return RedirectToAction("Login", "Account");

            var users = _context.Users.ToList();
            return View(users);
        }

        // GET: Sửa người dùng
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.UserID == id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Sửa người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(int id, User updatedUser)
        {
            if (id != updatedUser.UserID)
            {
                return NotFound();
            }

            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Phone = updatedUser.Phone;
            user.Address = updatedUser.Address;
            user.Role = updatedUser.Role;
            user.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            TempData["SuccessMessage"] = "✔️ Cập nhật người dùng thành công!";
            return RedirectToAction("Index");
        }

        // POST: Xóa người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserID == id);
            if (user == null)
                return NotFound();

            // Cần kiểm tra nếu có liên kết với Family hoặc Child không cho phép xóa thẳng tay
            var hasFamily = _context.Families.Any(f => f.PrimaryUserID == id);
            if (hasFamily)
            {
                TempData["ErrorMessage"] = "❌ Không thể xóa User này vì đang liên kết với một Family.";
                return RedirectToAction(nameof(Index));
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "🗑️ Đã xóa người dùng thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Danh sách lịch tiêm chủng
        public IActionResult ManageVaccinations()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var vaccinations = _context.VaccinationRecords
                .Include(v => v.Child)
                .OrderByDescending(v => v.VaccinationDate)
                .ToList();

            return View(vaccinations);
        }

        // GET: Sửa lịch tiêm
        [HttpGet]
        public IActionResult EditVaccination(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var vaccination = _context.VaccinationRecords
                .Include(v => v.Child)
                .FirstOrDefault(v => v.VaccinationID == id);

            if (vaccination == null)
                return NotFound();

            ViewBag.Children = _context.Children.ToList();
            return View(vaccination);
        }

        // POST: Sửa lịch tiêm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditVaccination(int id, VaccinationRecord updated)
        {
            if (id != updated.VaccinationID)
                return NotFound();

            var vaccination = _context.VaccinationRecords.Find(id);
            if (vaccination == null)
                return NotFound();

            vaccination.ChildID = updated.ChildID;
            vaccination.VaccineName = updated.VaccineName;
            vaccination.VaccinationDate = updated.VaccinationDate;
            vaccination.Status = updated.Status;
            vaccination.Notes = updated.Notes;
            vaccination.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            TempData["SuccessMessage"] = "✔️ Cập nhật lịch tiêm chủng thành công!";
            return RedirectToAction(nameof(ManageVaccinations));
        }

        // POST: Xóa lịch tiêm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteVaccination(int id)
        {
            var vaccination = _context.VaccinationRecords.Find(id);
            if (vaccination == null)
                return NotFound();

            _context.VaccinationRecords.Remove(vaccination);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "🗑️ Xóa lịch tiêm chủng thành công!";
            return RedirectToAction(nameof(ManageVaccinations));
        }

        // GET: Danh sách chỉ số phát triển
        public IActionResult ManageGrowthRecords()
        {
            var records = _context.GrowthRecords
                .Include(g => g.Child) 
                .OrderByDescending(g => g.RecordDate)
                .ToList();

            return View(records);
        }


        // GET: Sửa chỉ số
        [HttpGet]
        public IActionResult EditGrowthRecord(int id)
        {
            var record = _context.GrowthRecords.Find(id);
            if (record == null) return NotFound();

            ViewBag.Children = _context.Children
                .Select(c => new SelectListItem
                {
                    Value = c.ChildID.ToString(),
                    Text = c.FirstName + " " + c.LastName
                }).ToList();

            return View(record);
        }

        // POST: Sửa chỉ số
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditGrowthRecord(int id, GrowthRecord updated)
        {
            if (id != updated.GrowthRecordID) return BadRequest();

            ViewBag.Children = _context.Children
                .Select(c => new SelectListItem
                {
                    Value = c.ChildID.ToString(),
                    Text = c.FirstName + " " + c.LastName
                }).ToList();

            if (!ModelState.IsValid)
            {
                // Debug log
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("❌ Model Error: " + error.ErrorMessage);
                }

                return View(updated);
            }

            var record = _context.GrowthRecords.Find(id);
            if (record == null) return NotFound();

            record.ChildID = updated.ChildID;
            record.RecordDate = updated.RecordDate;
            record.Weight = updated.Weight;
            record.Height = updated.Height;
            record.HeadCircumference = updated.HeadCircumference;
            record.Notes = updated.Notes;
            record.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            TempData["SuccessMessage"] = "✔️ Đã cập nhật chỉ số phát triển!";
            return RedirectToAction(nameof(ManageGrowthRecords));
        }

        // POST: Xóa chỉ số
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteGrowthRecord(int id)
        {
            var record = _context.GrowthRecords.Find(id);
            if (record == null) return NotFound();

            _context.GrowthRecords.Remove(record);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "🗑️ Đã xóa chỉ số phát triển!";
            return RedirectToAction(nameof(ManageGrowthRecords));
        }


        // GET: Danh sách bác sĩ
        public IActionResult ManageDoctors()
        {
            var doctors = _context.Doctors
                .Include(d => d.User)
                .ToList();
            return View(doctors);
        }

        // GET: Thêm bác sĩ
        [HttpGet]
        public IActionResult CreateDoctor()
        {
            ViewBag.Users = _context.Users.ToList();
            return View();
        }

        // POST: Thêm bác sĩ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateDoctor(Doctor doctor)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Users = _context.Users.ToList();
                return View(doctor);
            }

            doctor.CreatedAt = DateTime.Now;
            doctor.UpdatedAt = DateTime.Now;

            _context.Doctors.Add(doctor);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "✔️ Thêm bác sĩ thành công!";
            return RedirectToAction(nameof(ManageDoctors));
        }

        // GET: Sửa bác sĩ
        [HttpGet]
        public IActionResult EditDoctor(int id)
        {
            var doctor = _context.Doctors.Find(id);
            if (doctor == null) return NotFound();

            ViewBag.Users = _context.Users.ToList();
            return View(doctor);
        }

        // POST: Sửa bác sĩ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDoctor(int id, Doctor updated)
        {
            if (id != updated.DoctorID)
                return NotFound();

            var doctor = _context.Doctors.Find(id);
            if (doctor == null) return NotFound();

            doctor.UserID = updated.UserID;
            doctor.Specialty = updated.Specialty;
            doctor.Hospital = updated.Hospital;
            doctor.Bio = updated.Bio;
            doctor.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            TempData["SuccessMessage"] = "✔️ Cập nhật bác sĩ thành công!";
            return RedirectToAction(nameof(ManageDoctors));
        }

        // POST: Xoá bác sĩ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDoctor(int id)
        {
            var doctor = _context.Doctors.Find(id);
            if (doctor == null) return NotFound();

            _context.Doctors.Remove(doctor);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "🗑️ Đã xoá bác sĩ.";
            return RedirectToAction(nameof(ManageDoctors));
        }

        // GET: Danh sách dịch vụ y tế
        public IActionResult ManageMedicalServices()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var services = _context.MedicalServices.OrderByDescending(s => s.CreatedAt).ToList();
            return View(services);
        }

        // GET: Tạo mới
        [HttpGet]
        public IActionResult CreateMedicalService()
        {
            return View();
        }

        // POST: Tạo mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateMedicalService(MedicalService service)
        {
            if (!ModelState.IsValid)
                return View(service);

            service.CreatedAt = DateTime.Now;
            service.UpdatedAt = DateTime.Now;

            _context.MedicalServices.Add(service);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "✔️ Đã thêm dịch vụ y tế.";
            return RedirectToAction(nameof(ManageMedicalServices));
        }

        // GET: Sửa
        [HttpGet]
        public IActionResult EditMedicalService(int id)
        {
            var service = _context.MedicalServices.Find(id);
            if (service == null)
                return NotFound();

            return View(service);
        }

        // POST: Sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMedicalService(int id, MedicalService updated)
        {
            if (id != updated.ServiceID)
                return BadRequest();

            var service = _context.MedicalServices.Find(id);
            if (service == null)
                return NotFound();

            service.Name = updated.Name;
            service.Type = updated.Type;
            service.Address = updated.Address;
            service.Phone = updated.Phone;
            service.Description = updated.Description;
            service.Rating = updated.Rating;
            service.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            TempData["SuccessMessage"] = "✔️ Đã cập nhật dịch vụ y tế.";
            return RedirectToAction(nameof(ManageMedicalServices));
        }

        // POST: Xóa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMedicalService(int id)
        {
            var service = _context.MedicalServices.Find(id);
            if (service == null)
                return NotFound();

            _context.MedicalServices.Remove(service);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "🗑️ Đã xoá dịch vụ y tế.";
            return RedirectToAction(nameof(ManageMedicalServices));
        }

        // GET: Danh sách chuyên gia
        public IActionResult ManageExperts()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");
            var experts = _context.Experts.Include(e => e.User).ToList(); // Include User ở đây
            return View(experts);
        }

        // GET: Thêm chuyên gia
        [HttpGet]
        public IActionResult CreateExpert()
        {
            var users = _context.Users
                .Where(u => u.Role == "Expert" && !_context.Experts.Any(e => e.UserID == u.UserID))
                .Select(u => new {
                    u.UserID,
                    FullName = u.FirstName + " " + u.LastName
                }).ToList();

            ViewBag.Users = new SelectList(users, "UserID", "FullName");

            return View();
        }



        // POST: Thêm chuyên gia
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExpert(Expert expert, IFormFile ImageFile)
        {
            ModelState.Remove("ProfilePicture");
            if (!ModelState.IsValid || ImageFile == null)
            {
                if (ImageFile == null)
                    ModelState.AddModelError("ImageFile", "Ảnh đại diện là bắt buộc.");

                // Nạp lại danh sách user
                var users = _context.Users
                    .Where(u => u.Role == "Expert" && !_context.Experts.Any(e => e.UserID == u.UserID))
                    .Select(u => new { u.UserID, FullName = u.FirstName + " " + u.LastName })
                    .ToList();

                ViewBag.Users = new SelectList(users, "UserID", "FullName");
                return View(expert);
            }

            // Tạo thư mục lưu nếu chưa có
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/experts");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Tạo tên file duy nhất
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Lưu file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(stream);
            }

            expert.ProfilePicture = "/images/experts/" + fileName;
            expert.CreatedAt = DateTime.Now;
            expert.UpdatedAt = DateTime.Now;
            _context.Experts.Add(expert);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "✔️ Thêm chuyên gia thành công!";
            return RedirectToAction(nameof(ManageExperts));
        }



        // GET: Sửa chuyên gia
        [HttpGet]
        public IActionResult EditExpert(int id)
        {
            var expert = _context.Experts.Find(id);
            if (expert == null) return NotFound();

            return View(expert);
        }

        // POST: Sửa chuyên gia
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditExpert(int id, Expert updated, IFormFile ImageFile)
        {
            if (id != updated.ExpertID)
                return BadRequest();

            var expert = _context.Experts.Find(id);
            if (expert == null) return NotFound();

            expert.Field = updated.Field;
            expert.Degree = updated.Degree;
            expert.ExperienceYears = updated.ExperienceYears;
            expert.Bio = updated.Bio;
            expert.UpdatedAt = DateTime.Now;

            // Nếu người dùng upload ảnh mới
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/experts");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                expert.ProfilePicture = "/images/experts/" + fileName;
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "✔️ Đã cập nhật chuyên gia!";
            return RedirectToAction(nameof(ManageExperts));
        }

        // POST: Xoá chuyên gia
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteExpert(int id)
        {
            var expert = _context.Experts.Find(id);
            if (expert == null) return NotFound();

            _context.Experts.Remove(expert);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "🗑️ Đã xoá chuyên gia!";
            return RedirectToAction(nameof(ManageExperts));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteArticle(int id)
        {
            var article = _context.Articles.Find(id);
            if (article == null)
                return NotFound();

            _context.Articles.Remove(article);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "🗑️ Đã xoá bài viết chuyên gia!";
            return RedirectToAction("ManageArticles"); // tuỳ vào trang bạn muốn quay về
        }

        public IActionResult ManageArticles()
        {
            var articles = _context.Articles
                .Include(a => a.Author)
                .Where(a => a.Category == "ChuyenGia") // nếu chỉ muốn lọc bài viết chuyên gia
                .ToList();

            return View(articles);
        }

        public IActionResult ManageContacts()
        {
            var messages = _context.ContactMessages.OrderByDescending(m => m.SentAt).ToList();
            return View(messages);
        }

        [HttpPost]
        public IActionResult Reply(int id, string adminReply)
        {
            var message = _context.ContactMessages.Find(id);
            if (message == null) return NotFound();

            message.AdminReply = adminReply;
            message.RepliedAt = DateTime.Now;
            _context.SaveChanges();

            TempData["Success"] = "Đã gửi phản hồi đến người dùng!";
            return RedirectToAction("ManageContacts");
        }

    }
}
