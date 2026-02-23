using MeBeCare.Models;
using MeBeCare.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ExpertController : Controller
{
    private readonly AppDbContext _context;

    public ExpertController(AppDbContext context)
    {
        _context = context;
    }

    // Trang Dashboard cho chuyên gia
    public IActionResult Dashboard()
    {
        var userId = HttpContext.Session.GetInt32("UserID");
        var role = HttpContext.Session.GetString("UserRole");

        if (userId == null || role != "Expert")
            return RedirectToAction("Login", "Account");

        var growthRecords = _context.GrowthRecords
            .Include(gr => gr.Child)
            .ThenInclude(c => c.Family)
            .ThenInclude(f => f.PrimaryUser)
            .ToList();

        var articles = _context.Articles
            .Where(a => a.AuthorID == userId)
            .ToList();

        var model = new ExpertDashboardViewModel
        {
            GrowthRecords = growthRecords,
            Articles = articles
        };

        return View(model);
    }

    // GET: Viết bài
    [HttpGet]
    public IActionResult CreateArticle()
    {
        return View();
    }

    // POST: Viết bài
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateArticle(Article model, IFormFile? ImageFile, IFormFile? VideoFile)
    {
        var userId = HttpContext.Session.GetInt32("UserID");
        if (userId == null)
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            // Xử lý ảnh
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var imageFolder = Path.Combine("wwwroot/images/articles");
                Directory.CreateDirectory(imageFolder);
                var imageFileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var imagePath = Path.Combine(imageFolder, imageFileName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                model.ImageUrl = "/images/articles/" + imageFileName;
            }

            // Xử lý video
            if (VideoFile != null && VideoFile.Length > 0)
            {
                var videoFolder = Path.Combine("wwwroot/videos/articles");
                Directory.CreateDirectory(videoFolder);
                var videoFileName = Guid.NewGuid() + Path.GetExtension(VideoFile.FileName);
                var videoPath = Path.Combine(videoFolder, videoFileName);
                using (var stream = new FileStream(videoPath, FileMode.Create))
                {
                    await VideoFile.CopyToAsync(stream);
                }
                model.VideoUrl = "/videos/articles/" + videoFileName;
            }

            model.AuthorID = userId.Value;
            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;

            _context.Articles.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "✅ Bài viết đã được đăng thành công!";
            return RedirectToAction("Dashboard");
        }
        catch (Exception ex)
        {
            var errorMsg = ex.InnerException?.Message ?? ex.Message;
            ModelState.AddModelError("", $"❌ Đã xảy ra lỗi khi đăng bài: {errorMsg}");
            return View(model);
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserID");
        if (userId == null) return RedirectToAction("Login", "Account");

        var article = await _context.Articles.FirstOrDefaultAsync(a => a.ArticleID == id && a.AuthorID == userId);

        if (article == null) return NotFound();

        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "🗑️ Bài viết đã được xoá thành công!";
        return RedirectToAction("Dashboard");
    }

    // GET: Chỉnh sửa bài viết
    [HttpGet]
    public IActionResult EditArticle(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserID");
        var role = HttpContext.Session.GetString("UserRole");

        if (userId == null || role != "Expert") return RedirectToAction("Login", "Account");

        var article = _context.Articles.FirstOrDefault(a => a.ArticleID == id && a.AuthorID == userId);
        if (article == null) return NotFound();

        return View(article);
    }

    // POST: Lưu bài viết sau khi chỉnh sửa
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditArticle(Article model, IFormFile? ImageFile, IFormFile? VideoFile)
    {
        var userId = HttpContext.Session.GetInt32("UserID");
        if (userId == null || model.AuthorID != userId) return Unauthorized();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var existing = _context.Articles.FirstOrDefault(a => a.ArticleID == model.ArticleID && a.AuthorID == userId);
            if (existing == null) return NotFound();

            existing.Title = model.Title;
            existing.Content = model.Content;
            existing.UpdatedAt = DateTime.Now;

            // Nếu có file ảnh mới
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var imgPath = Path.Combine("wwwroot/images/articles");
                Directory.CreateDirectory(imgPath);
                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(imgPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await ImageFile.CopyToAsync(stream);

                existing.ImageUrl = "/images/articles/" + fileName;
            }

            // Nếu có file video mới
            if (VideoFile != null && VideoFile.Length > 0)
            {
                var videoPath = Path.Combine("wwwroot/videos/articles");
                Directory.CreateDirectory(videoPath);
                var fileName = Guid.NewGuid() + Path.GetExtension(VideoFile.FileName);
                var fullPath = Path.Combine(videoPath, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                    await VideoFile.CopyToAsync(stream);

                existing.VideoUrl = "/videos/articles/" + fileName;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "✅ Cập nhật bài viết thành công!";
            return RedirectToAction("Dashboard");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"❌ Có lỗi khi cập nhật bài viết: {ex.Message}");
            return View(model);
        }
    }
}
