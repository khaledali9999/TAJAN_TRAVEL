using Microsoft.AspNetCore.Mvc;
using TAJAN_TRAVEL.Data;

namespace TAJAN_TRAVEL.Areas.Admin.Controllers
{
    // Placeholder for sidebar sections that are not built yet.
    public class ComingSoonController : AdminBaseController
    {
        public ComingSoonController(ApplicationDbContext db) : base(db) { }

        public IActionResult Index(string? title)
        {
            ViewData["Title"] = string.IsNullOrWhiteSpace(title) ? "Coming soon" : title.Trim();
            return View();
        }
    }
}
