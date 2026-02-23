using Microsoft.AspNetCore.Mvc;
using MeBeCare.Models;
using MeBeCare.Data;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;

namespace MeBeCare.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng. Vui lòng chọn email khác.");
                return View(user);
            }

            try
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                user.CreatedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;

                if (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))
                {
                    ModelState.AddModelError("", "Vui lòng nhập đầy đủ Họ và Tên.");
                    return View(user);
                }

                if (string.IsNullOrEmpty(user.Role))
                {
                    user.Role = "Mother"; // Mặc định là Mother nếu chưa chọn
                }

                if (user.Role == "Chuyên gia")
                {
                    // KHÔNG tạo Expert ở đây!
                    // Admin sẽ chọn từ danh sách và thêm thủ công
                }


                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", $"Đã xảy ra lỗi khi đăng ký: {innerMessage}");
                return View(user);
            }

        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                HttpContext.Session.SetInt32("UserID", user.UserID);
                HttpContext.Session.SetString("FullName", $"{user.FirstName} {user.LastName}");
                HttpContext.Session.SetString("UserRole", user.Role); // Gán thêm Role vào Session

                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin"); // Nếu Admin => vào trang Admin
                }
                else if (user.Role == "Expert")
                {
                    return RedirectToAction("Dashboard", "Expert"); // 👉 chuyển đúng về ExpertController
                }
                else
                {
                    return RedirectToAction("Dashboard", "Home");
                }
            }

            ViewBag.Error = "Sai email hoặc mật khẩu.";
            return View();
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
