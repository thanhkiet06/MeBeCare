using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MeBeCare.Data;
using MeBeCare.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MeBeCare.Controllers
{
    public class MedicalRecordController : Controller
    {
        private readonly AppDbContext _context;

        public MedicalRecordController(AppDbContext context)
        {
            _context = context;
        }

        // Danh sách
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var records = await _context.MedicalRecords
            .Include(r => r.Child)
            .Where(m => m.UserID == userId || m.Child.Family.PrimaryUserID == userId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

            return View(records);
        }

        // Tạo mới
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Children = _context.Children
                .Where(c => c.Family.PrimaryUserID == userId)
                .ToList();

            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalRecord record)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // ✅ Nếu không chọn bé => lưu cho mẹ
            if (record.ChildID == null)
            {
                record.UserID = userId.Value;
            }

            record.CreatedAt = DateTime.Now;
            record.UpdatedAt = DateTime.Now;

            _context.MedicalRecords.Add(record);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "📝 Lưu hồ sơ thành công!";
            return RedirectToAction("Index");
        }


        // Sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedicalRecord record)
        {
            if (id != record.MedicalRecordID)
                return NotFound();

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("Model error: " + error.ErrorMessage);
                }

                // Load lại danh sách Children để hiển thị lại dropdown
                ViewBag.Children = _context.Children
                    .Where(c => c.Family.PrimaryUserID == HttpContext.Session.GetInt32("UserID"))
                    .ToList();

                return View(record);
            }

            var existingRecord = await _context.MedicalRecords.FindAsync(id);
            if (existingRecord == null)
                return NotFound();

            existingRecord.RecordType = record.RecordType;
            existingRecord.Description = record.Description;
            existingRecord.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "✔️ Hồ sơ đã được cập nhật!";
            return RedirectToAction("Index");
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Children = _context.Children
                .Where(c => c.Family.PrimaryUserID == userId)
                .ToList();

            var record = await _context.MedicalRecords.FindAsync(id);
            if (record == null)
            {
                return NotFound();
            }

            return View(record);
        }



        // Xoá
        public async Task<IActionResult> Delete(int id)
        {
            var record = await _context.MedicalRecords.FindAsync(id);
            if (record == null) return NotFound();

            _context.MedicalRecords.Remove(record);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
