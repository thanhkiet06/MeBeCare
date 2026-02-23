using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MeBeCare.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MeBeCare.Data;

namespace MeBeCare.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context; // ✅ THÊM dòng này

        // ✅ THAY ĐỔI constructor để nhận AppDbContext
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // ✅ Hồ sơ sức khỏe
        public async Task<IActionResult> MedicalRecords()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var records = await _context.MedicalRecords
                .Where(r => r.UserID == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(records);
        }

        // Trang chủ
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ThaiKyRedirect()
        {
            return RedirectToAction("Index", "ThaiKy");
        }

        public IActionResult TinhNang()
        {
            return View();
        }

        public IActionResult HuongDan()
        {
            return View();
        }

        public IActionResult LienHe()
        {
            return View("LienHe"); 
        }

        public IActionResult Dashboard()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserID")))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.FullName = HttpContext.Session.GetString("FullName");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult Support()
        {
            return View();
        }

        public IActionResult ChuyenGia()
        {
            var experts = _context.Experts.Include(e => e.User).ToList();
            return View(experts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        public IActionResult ExpertDetail(int id)
        {
            var expert = _context.Experts
                .Include(e => e.User)
                .FirstOrDefault(e => e.ExpertID == id);

            if (expert == null) return NotFound();
            return View("ChuyenGiaChiTiet", expert);
        }

        public IActionResult ExpertArticles(int id)
        {
            var articles = _context.Articles
                .Where(a => a.AuthorID == id && a.Category == "ChuyenGia")
                .OrderByDescending(a => a.CreatedAt)
                .ToList();

            var author = _context.Users.FirstOrDefault(u => u.UserID == id);
            ViewBag.AuthorName = $"{author?.FirstName} {author?.LastName}";

            return View("ChuyenGiaBaiViet", articles);
        }

        public IActionResult ArticleDetail(int id)
        {
            var article = _context.Articles
                .Include(a => a.Author)
                .FirstOrDefault(a => a.ArticleID == id);

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }
    }
}
