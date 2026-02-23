using Microsoft.AspNetCore.Mvc;
using MeBeCare.Data;
using MeBeCare.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MeBeCare.Controllers

{
    public class ChildController : Controller
    {
        private readonly AppDbContext _context;

        public ChildController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var children = _context.Children
                .Where(c => c.Family.PrimaryUserID == userId)
                .ToList();

            return View(children);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Child child)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Kiểm tra ngày sinh hợp lệ
            if (child.DateOfBirth > DateTime.Now)
            {
                ModelState.AddModelError("DateOfBirth", "Ngày sinh không được nằm trong tương lai.");
            }

            if (ModelState.IsValid)
            {
                // Tìm hoặc tạo Family
                var family = await _context.Families.FirstOrDefaultAsync(f => f.PrimaryUserID == userId);
                if (family == null)
                {
                    family = new Family
                    {
                        PrimaryUserID = userId.Value,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Families.Add(family);
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "Lỗi khi tạo gia đình: " + ex.Message;
                        return View(child);
                    }
                }

                child.FamilyID = family.FamilyID;
                child.CreatedAt = DateTime.Now;
                child.UpdatedAt = DateTime.Now;

                _context.Children.Add(child);
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Thêm bé thành công!";
                    return RedirectToAction("Index", "MedicalRecord");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Lỗi khi lưu bé: " + ex.Message;
                    return View(child);
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            TempData["ErrorMessage"] = "Vui lòng kiểm tra thông tin nhập: " + string.Join(", ", errors);
            return View(child);
        }

        // Phương thức lấy UserID từ Claims (đã có từ MedicalRecordController)
        private async Task<int?> GetCurrentUserIdAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return null;
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            return null;
        }
    }
}
