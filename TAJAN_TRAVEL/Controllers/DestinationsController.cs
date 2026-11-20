using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.ViewModels;

namespace TAJAN_TRAVEL.Controllers
{
    public class DestinationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DestinationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /Destinations?page=2
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 6;
            if (page < 1) page = 1;

            var query = _context.Destinations.Where(d => d.IsActive);

            var totalCount = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
            page = Math.Min(page, totalPages);

            var destinations = await query
                .OrderBy(d => d.DisplayOrder)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vm = new DestinationListViewModel
            {
                Destinations = destinations,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            // Pagination clicks fetch this same URL via JS (X-Requested-With header) so the
            // page doesn't reload - in that case send back just the grid + pagination markup.
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_DestinationsResults", vm);

            vm.FeaturedTours = await _context.Tours
                .Include(t => t.Destination)
                .Where(t => t.IsActive && t.IsFeatured)
                .OrderByDescending(t => t.CreatedAt)
                .Take(6)
                .ToListAsync();

            return View(vm);
        }

        [Route("Destinations/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            var destination = await _context.Destinations
                .Include(d => d.Tours.Where(t => t.IsActive))
                .FirstOrDefaultAsync(d => d.Slug == slug && d.IsActive);

            if (destination == null)
                return NotFound();

            return View(destination);
        }
    }
}
