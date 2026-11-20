using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.Entities;
using TAJAN_TRAVEL.Models.ViewModels;

namespace TAJAN_TRAVEL.Controllers
{
    public class EventsController : Controller
    {
        private const int PageSize = 6;
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /Events
        // GET /Events?category=festivals
        // GET /Events?thisMonth=true
        // GET /Events?page=2
        public async Task<IActionResult> Index(string? category, bool thisMonth = false, int page = 1)
        {
            if (page < 1) page = 1;

            var active = _context.TravelEvents
                .Include(e => e.Category)
                .Where(e => e.IsActive);

            var isFiltered = !string.IsNullOrWhiteSpace(category) || thisMonth;

            // The big "Featured Event" card is only shown on the unfiltered list.
            TravelEvent? featured = null;
            if (!isFiltered)
            {
                featured = await active
                    .Where(e => e.IsFeatured)
                    .OrderBy(e => e.DisplayOrder)
                    .FirstOrDefaultAsync();
            }

            var query = active;

            if (featured != null)
                query = query.Where(e => e.Id != featured.Id);

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(e => e.Category != null && e.Category.Slug == category);

            if (thisMonth)
            {
                var now = DateTime.UtcNow;
                var start = new DateTime(now.Year, now.Month, 1);
                var end = start.AddMonths(1);
                query = query.Where(e => e.EventDate >= start && e.EventDate < end);
            }

            query = query.OrderBy(e => e.DisplayOrder).ThenBy(e => e.EventDate);

            var total = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
            page = Math.Min(page, totalPages);

            var vm = new EventsViewModel
            {
                Featured = featured,
                Events = await query.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync(),
                Categories = await _context.EventCategories.OrderBy(c => c.DisplayOrder).ToListAsync(),
                Category = category,
                ThisMonth = thisMonth,
                CurrentPage = page,
                TotalPages = totalPages
            };

            // Filter chips / pagination fetch this same URL via JS: return only the results block.
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                Response.Headers["Vary"] = "X-Requested-With"; // don't let the browser cache the fragment as the full page
                return PartialView("_EventsResults", vm);
            }

            return View(vm);
        }
    }
}
