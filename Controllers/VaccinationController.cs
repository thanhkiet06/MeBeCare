using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeBeCare.Data;
using MeBeCare.Models;

namespace MeBeCare.Controllers
{
    public class VaccinationController : Controller
    {
        private readonly AppDbContext _context;

        public VaccinationController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var vaccinations = await _context.VaccinationRecords
                .Include(v => v.Child)
                .Where(v => v.Child.Family.PrimaryUserID == userId)
                .OrderByDescending(v => v.VaccinationDate)
                .ToListAsync();

            return View(vaccinations);
        }

        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var children = _context.Children
                .Include(c => c.Family)
                .Where(c => c.Family.PrimaryUserID == userId)
                .ToList();

            if (!children.Any())
            {
                TempData["ErrorMessage"] = "Vui lòng thêm thông tin trẻ trước khi tạo bản ghi tiêm chủng.";
                return RedirectToAction("Index");
            }

            ViewBag.Children = children;
            return View(new VaccinationRecord());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VaccinationRecord record)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Loại bỏ validation cho Child để chỉ kiểm tra ChildID
            ModelState.Remove("Child");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));

                ViewBag.Children = await _context.Children
                    .Include(c => c.Family)
                    .Where(c => c.Family.PrimaryUserID == userId)
                    .ToListAsync();

                return View(record);
            }

            try
            {
                // Kiểm tra ChildID hợp lệ
                var child = await _context.Children
                    .Include(c => c.Family)
                    .Where(c => c.ChildID == record.ChildID && c.Family != null && c.Family.PrimaryUserID == userId)
                    .FirstOrDefaultAsync();

                if (child == null)
                {
                    ModelState.AddModelError("ChildID", "Trẻ được chọn không hợp lệ hoặc không thuộc gia đình của bạn.");
                    ViewBag.Children = await _context.Children
                        .Include(c => c.Family)
                        .Where(c => c.Family.PrimaryUserID == userId)
                        .ToListAsync();
                    return View(record);
                }

                // Đảm bảo Status hợp lệ
                if (!new[] { "Completed", "Pending", "Missed" }.Contains(record.Status))
                {
                    ModelState.AddModelError("Status", "Trạng thái phải là Completed, Pending hoặc Missed.");
                    ViewBag.Children = await _context.Children
                        .Include(c => c.Family)
                        .Where(c => c.Family.PrimaryUserID == userId)
                        .ToListAsync();
                    return View(record);
                }

                // Cập nhật thời gian
                record.CreatedAt = DateTime.Now;
                record.UpdatedAt = DateTime.Now;

                _context.VaccinationRecords.Add(record);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Lưu lịch tiêm chủng thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving to database: " + ex.Message);
                ModelState.AddModelError("", $"Đã xảy ra lỗi khi lưu dữ liệu: {ex.Message}");
                ViewBag.Children = await _context.Children
                    .Include(c => c.Family)
                    .Where(c => c.Family.PrimaryUserID == userId)
                    .ToListAsync();
                return View(record);
            }
        }

        // GET: Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Login", "Account");

            var record = await _context.VaccinationRecords
                .Include(v => v.Child)
                .FirstOrDefaultAsync(v => v.VaccinationID == id && v.Child.Family.PrimaryUserID == userId);

            if (record == null) return NotFound();

            ViewBag.Children = _context.Children
                .Where(c => c.Family.PrimaryUserID == userId)
                .ToList();

            return View(record);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VaccinationRecord record)
        {
            if (id != record.VaccinationID) return NotFound();

            ModelState.Remove("Child");

            if (!ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("UserID");
                ViewBag.Children = _context.Children
                    .Where(c => c.Family.PrimaryUserID == userId)
                    .ToList();
                return View(record);
            }

            try
            {
                var existing = await _context.VaccinationRecords.FindAsync(id);
                if (existing == null) return NotFound();

                existing.VaccineName = record.VaccineName;
                existing.VaccinationDate = record.VaccinationDate;
                existing.Status = record.Status;
                existing.Notes = record.Notes;
                existing.ChildID = record.ChildID;
                existing.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật thành công!";
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật bản ghi.");
                return View(record);
            }
        }

        // DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var record = await _context.VaccinationRecords.FindAsync(id);
            if (record == null) return NotFound();

            _context.VaccinationRecords.Remove(record);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xoá thành công!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> WeeklySchedule()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var records = await _context.VaccinationRecords
                .Include(v => v.Child)
                .Where(v => v.Child.Family.PrimaryUserID == userId)
                .OrderBy(v => v.VaccinationDate)
                .ToListAsync();

            return View(records); // tạo WeeklySchedule.cshtml
        }
        public async Task<IActionResult> SuggestVaccines()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var today = DateTime.Today;

            var children = await _context.Children
                .Include(c => c.Family)
                .Where(c => c.Family.PrimaryUserID == userId)
                .ToListAsync();

            var suggestions = new List<(string ChildName, int AgeInWeeks, List<string> RecommendedVaccines)>();

            foreach (var child in children)
            {
                int ageInWeeks = (int)((today - child.DateOfBirth).TotalDays / 7);

                var recommendedVaccines = new List<string>();

                if (ageInWeeks < 1)
                {
                    recommendedVaccines.Add("Viêm gan B mũi 1");
                }
                else if (ageInWeeks < 4)
                {
                    recommendedVaccines.Add("BCG (lao)");
                    recommendedVaccines.Add("Viêm gan B mũi 2");
                }
                else if (ageInWeeks < 8)
                {
                    recommendedVaccines.Add("6 trong 1 mũi 1 (bạch hầu, ho gà, uốn ván, bại liệt, Hib, viêm gan B)");
                    recommendedVaccines.Add("Phế cầu mũi 1");
                    recommendedVaccines.Add("Rota mũi 1");
                }
                else if (ageInWeeks < 12)
                {
                    recommendedVaccines.Add("6 trong 1 mũi 2");
                    recommendedVaccines.Add("Rota mũi 2");
                    recommendedVaccines.Add("Phế cầu mũi 2");
                }
                else if (ageInWeeks < 16)
                {
                    recommendedVaccines.Add("6 trong 1 mũi 3");
                    recommendedVaccines.Add("Phế cầu mũi 3");
                }
                else if (ageInWeeks < 24)
                {
                    recommendedVaccines.Add("Cúm mùa mũi 1");
                }
                else if (ageInWeeks < 36)
                {
                    recommendedVaccines.Add("Sởi - Quai bị - Rubella (MMR)");
                    recommendedVaccines.Add("Phế cầu nhắc lại");
                }
                else if (ageInWeeks < 52)
                {
                    recommendedVaccines.Add("Viêm gan A");
                    recommendedVaccines.Add("Thủy đậu");
                }
                else
                {
                    recommendedVaccines.Add("Tiếp tục tiêm nhắc các vắc xin theo lịch mở rộng hoặc theo khuyến cáo của bác sĩ.");
                }

                suggestions.Add((child.FirstName + " " + child.LastName, ageInWeeks, recommendedVaccines));
            }

            ViewBag.Suggestions = suggestions;
            return View("SuggestVaccines"); // bạn sẽ tạo view này
        }

    }
}