using TAJAN_TRAVEL.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.Entities;
using TAJAN_TRAVEL.Models.ViewModels;

namespace TAJAN_TRAVEL.Controllers
{
    public class CruisesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CruisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cruises = await _context.Tours
                .Include(t => t.Destination)
                .Where(t => t.IsActive && t.Type == TourType.NileCruise)
                .OrderByDescending(t => t.IsFeatured)
                .ToListAsync();

            var vm = new TourListViewModel
            {
                Tours = cruises,
                PageTitle = Lang.T("Nile Cruises"),
                PageSubtitle = Lang.T("Sail through ancient history on private feluccas and luxury river cruises.")
            };

            return View(vm);
        }

        [Route("Cruises/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            var cruise = await _context.Tours
                .Include(t => t.Destination)
                .Include(t => t.Images.OrderBy(i => i.DisplayOrder))
                .Include(t => t.Itinerary.OrderBy(i => i.DayNumber))
                .FirstOrDefaultAsync(t => t.Slug == slug && t.IsActive && t.Type == TourType.NileCruise);

            if (cruise == null)
                return NotFound();

            var vm = new TourDetailsViewModel { Tour = cruise };
            return View(vm);
        }
    }
}
