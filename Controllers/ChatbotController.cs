using Microsoft.AspNetCore.Mvc;

public class ChatbotController : Controller
{
    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetInt32("UserID");
        if (userId == null)
            return RedirectToAction("Login", "Account");

        return View(); // Trả về Views/Chatbot/Index.cshtml
    }
}
