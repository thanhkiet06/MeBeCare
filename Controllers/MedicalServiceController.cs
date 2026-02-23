using Microsoft.AspNetCore.Mvc;
using MeBeCare.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MeBeCare.Controllers
{
    public class MedicalServiceController : Controller
    {
        private readonly AppDbContext _context;

        public MedicalServiceController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, string typeFilter)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var services = _context.MedicalServices.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                services = services.Where(s => s.Name.Contains(searchString) || s.Type.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(typeFilter) && typeFilter != "All")
            {
                services = services.Where(s => s.Type == typeFilter);
            }

            ViewBag.TypeFilter = typeFilter;
            ViewBag.SearchString = searchString;
            return View(await services.OrderByDescending(s => s.Rating).ToListAsync());
        }
    }
}
