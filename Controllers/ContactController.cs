using MeBeCare.Data;
using MeBeCare.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeBeCare.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitContact([FromForm] ContactMessage contact)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dữ liệu không hợp lệ");

            contact.SentAt = DateTime.Now;
            _context.ContactMessages.Add(contact);
            _context.SaveChanges();

            return Ok("Gửi thành công");
        }


        // ADMIN xem tất cả liên hệ
        public IActionResult Manage()
        {
            var messages = _context.ContactMessages.OrderByDescending(c => c.SentAt).ToList();
            return View(messages);
        }

        // ADMIN trả lời
        [HttpPost]
        public IActionResult Reply(int id, string reply)
        {
            var message = _context.ContactMessages.Find(id);
            if (message == null) return NotFound();

            message.AdminReply = reply;
            message.RepliedAt = DateTime.Now;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Đã gửi phản hồi cho người dùng.";
            return RedirectToAction("Manage");
        }
    }

}
