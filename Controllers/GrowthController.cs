using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeBeCare.Data;
using MeBeCare.Models;

namespace MeBeCare.Controllers
{
    public class GrowthController : Controller
    {
        private readonly AppDbContext _context;

        public GrowthController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var records = await _context.GrowthRecords
                .Include(g => g.Child)
                .Where(g => g.Child.Family.PrimaryUserID == userId)
                .OrderByDescending(g => g.RecordDate)
                .ToListAsync();

            return View(records);
        }

        // Hiển thị form thêm mới
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var children = _context.Children
                .Where(c => c.Family.PrimaryUserID == userId)
                .ToList();

            if (!children.Any())
            {
                TempData["ErrorMessage"] = "Vui lòng thêm thông tin trẻ trước.";
                return RedirectToAction("Index");
            }

            ViewBag.Children = children;
            return View(new GrowthRecord());
        }

        // Xử lý thêm mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GrowthRecord record)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            ModelState.Remove("Child");

            if (!ModelState.IsValid)
            {
                ViewBag.Children = _context.Children
                    .Where(c => c.Family.PrimaryUserID == userId)
                    .ToList();
                return View(record);
            }

            try
            {
                record.CreatedAt = DateTime.Now;
                record.UpdatedAt = DateTime.Now;

                _context.GrowthRecords.Add(record);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "✔️ Đã thêm chỉ số phát triển thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                ViewBag.Children = _context.Children
                    .Where(c => c.Family.PrimaryUserID == userId)
                    .ToList();
                return View(record);
            }
        }

        // Hiển thị form sửa
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var record = await _context.GrowthRecords
                .Include(g => g.Child)
                .FirstOrDefaultAsync(r => r.GrowthRecordID == id && r.Child.Family.PrimaryUserID == userId);

            if (record == null)
                return NotFound();

            ViewBag.Children = _context.Children
                .Where(c => c.Family.PrimaryUserID == userId)
                .ToList();

            return View(record);
        }

        // Xử lý sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GrowthRecord updated)
        {
            if (id != updated.GrowthRecordID)
                return BadRequest();

            var record = await _context.GrowthRecords.FindAsync(id);
            if (record == null)
                return NotFound();

            record.ChildID = updated.ChildID;
            record.RecordDate = updated.RecordDate;
            record.Weight = updated.Weight;
            record.Height = updated.Height;
            record.HeadCircumference = updated.HeadCircumference;
            record.Notes = updated.Notes;
            record.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "✔️ Đã cập nhật chỉ số phát triển!";
            return RedirectToAction("Index");
        }

        // Xóa bản ghi
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var record = await _context.GrowthRecords.FindAsync(id);
            if (record == null)
                return NotFound();

            _context.GrowthRecords.Remove(record);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "🗑️ Đã xóa chỉ số thành công!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Stats(int? childId)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var children = await _context.Children
                .Where(c => c.Family.PrimaryUserID == userId)
                .ToListAsync();
            ViewBag.Children = children;

            if (childId == null && children.Any())
            {
                childId = children.First().ChildID;
            }

            var selectedChild = await _context.Children
                .Include(c => c.GrowthRecords)
                .FirstOrDefaultAsync(c => c.ChildID == childId && c.Family.PrimaryUserID == userId);

            if (selectedChild == null || !selectedChild.GrowthRecords.Any())
            {
                ViewBag.Message = "Chưa có dữ liệu chỉ số phát triển.";
                return View(new List<GrowthRecord>());
            }

            return View(selectedChild.GrowthRecords.OrderBy(r => r.RecordDate).ToList());
        }

        public async Task<IActionResult> NutritionSuggestions(int? childId)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var children = await _context.Children
                .Where(c => c.Family.PrimaryUserID == userId)
                .ToListAsync();
            ViewBag.Children = children;
            ViewBag.SelectedChildId = childId;

            if (childId == null)
                return View(new List<string>());

            var child = await _context.Children.FindAsync(childId);
            if (child == null)
            {
                ViewBag.Message = "Không tìm thấy thông tin bé.";
                return View(new List<string>());
            }

            var ageInMonths = (int)((DateTime.Now - child.DateOfBirth).TotalDays / 30.44);
            var recommendations = new List<string>();

            if (ageInMonths < 6)
            {
                recommendations.Add("Chỉ bú mẹ hoàn toàn.");
                recommendations.Add("Bổ sung vitamin D nếu cần.");
            }
            else if (ageInMonths < 12)
            {
                recommendations.Add("Tiếp tục bú mẹ + ăn dặm.");
                recommendations.Add("Ăn cháo, rau nghiền, thịt xay.");
            }
            else if (ageInMonths < 24)
            {
                recommendations.Add("Ăn 3 bữa chính + 2 bữa phụ mỗi ngày.");
                recommendations.Add("Tăng cường canxi, sắt, kẽm qua thực phẩm.");
            }
            else
            {
                recommendations.Add("Chế độ ăn đa dạng: cơm, thịt, cá, rau, trái cây.");
                recommendations.Add("Hạn chế bánh kẹo, nước ngọt.");
            }

            return View(recommendations);
        }
    }
}