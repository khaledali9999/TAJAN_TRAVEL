using TAJAN_TRAVEL.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.Entities;
using TAJAN_TRAVEL.Models.ViewModels;

namespace TAJAN_TRAVEL.Controllers
{
    public class ToursController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ToursController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /Tours?destinationId=&activityTypeId=&type=&minDuration=&maxDuration=&maxPrice=&page=
        public async Task<IActionResult> Index(TourFilterViewModel filter, int page = 1)
        {
            const int pageSize = 6;
            if (page < 1) page = 1;

            var query = _context.Tours
                .Include(t => t.Destination)
                .Include(t => t.ActivityType)
                .Where(t => t.IsActive && t.Type != TourType.NileCruise)
                .AsQueryable();

            // Home-page search bar sends ranges like "4-7" / "2000+" - convert them to min/max
            if (TryParseRange(filter.Duration, out var dMin, out var dMax))
            {
                filter.MinDuration ??= (int?)dMin;
                filter.MaxDuration ??= (int?)dMax;
            }
            if (TryParseRange(filter.Price, out var pMin, out var pMax))
            {
                filter.MinPrice ??= pMin;
                filter.MaxPrice ??= pMax;
            }

            if (filter.DestinationId.HasValue)
                query = query.Where(t => t.DestinationId == filter.DestinationId.Value);

            if (filter.ActivityTypeId.HasValue)
                query = query.Where(t => t.ActivityTypeId == filter.ActivityTypeId.Value);

            if (filter.Type.HasValue)
                query = query.Where(t => t.Type == filter.Type.Value);

            if (filter.MinDuration.HasValue)
                query = query.Where(t => t.DurationDays >= filter.MinDuration.Value);

            if (filter.MaxDuration.HasValue)
                query = query.Where(t => t.DurationDays <= filter.MaxDuration.Value);

            if (filter.MinPrice.HasValue)
                query = query.Where(t => t.PriceFrom >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(t => t.PriceFrom <= filter.MaxPrice.Value);

            // Duration checkboxes (any ticked box matches): "1" = day trips only
            if (filter.DurationRanges.Count > 0)
            {
                bool d1 = filter.DurationRanges.Contains("1");
                bool d13 = filter.DurationRanges.Contains("1-3");
                bool d47 = filter.DurationRanges.Contains("4-7");
                bool d814 = filter.DurationRanges.Contains("8-14");
                bool d15 = filter.DurationRanges.Contains("15-plus");

                if (d1 || d13 || d47 || d814 || d15)
                {
                    query = query.Where(t =>
                        (d1 && t.DurationDays == 1) ||
                        (d13 && t.DurationDays >= 1 && t.DurationDays <= 3) ||
                        (d47 && t.DurationDays >= 4 && t.DurationDays <= 7) ||
                        (d814 && t.DurationDays >= 8 && t.DurationDays <= 14) ||
                        (d15 && t.DurationDays >= 15));
                }
            }

            query = query.OrderByDescending(t => t.IsFeatured)
                .ThenByDescending(t => t.CreatedAt);

            var totalCount = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
            page = Math.Min(page, totalPages);

            var tours = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            filter.Destinations = await _context.Destinations
                .Where(d => d.IsActive)
                .OrderBy(d => d.DisplayOrder)
                .ToListAsync();

            filter.ActivityTypes = await _context.ActivityTypes.ToListAsync();

            var vm = new TourListViewModel
            {
                Tours = tours,
                Filter = filter,
                PageTitle = Lang.T("Our Tours"),
                PageSubtitle = Lang.T("Private, tailor-made journeys across Egypt."),
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            // Pagination clicks fetch this same URL via JS (X-Requested-With header) so the
            // page doesn't reload - in that case send back just the grid + pagination markup.
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_ToursResults", vm);

            return View(vm);
        }

        private static bool TryParseRange(string? value, out decimal? min, out decimal? max)
        {
            min = null; max = null;
            if (string.IsNullOrWhiteSpace(value)) return false;
            var v = value.Trim();
            var inv = System.Globalization.CultureInfo.InvariantCulture;
            if (v.EndsWith('+'))
            {
                if (!decimal.TryParse(v.TrimEnd('+'), System.Globalization.NumberStyles.Number, inv, out var lo)) return false;
                min = lo;
                return true;
            }
            var parts = v.Split('-');
            if (parts.Length != 2 ||
                !decimal.TryParse(parts[0], System.Globalization.NumberStyles.Number, inv, out var a) ||
                !decimal.TryParse(parts[1], System.Globalization.NumberStyles.Number, inv, out var b)) return false;
            min = a; max = b;
            return true;
        }

        // GET /Tours/pyramids-egyptian-museum-tour
        [Route("Tours/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            var tour = await _context.Tours
                .Include(t => t.Destination)
                .Include(t => t.ActivityType)
                .Include(t => t.Images.OrderBy(i => i.DisplayOrder))
                .Include(t => t.Itinerary.OrderBy(i => i.DayNumber))
                .Include(t => t.Prices.OrderBy(p => p.Persons))
                .FirstOrDefaultAsync(t => t.Slug == slug && t.IsActive);

            if (tour == null)
                return NotFound();

            var related = await _context.Tours
                .Where(t => t.Id != tour.Id && t.DestinationId == tour.DestinationId && t.IsActive)
                .Take(3)
                .ToListAsync();

            var vm = new TourDetailsViewModel
            {
                Tour = tour,
                RelatedTours = related
            };

            return View(vm);
        }
    }
}