using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.Entities;
using TAJAN_TRAVEL.Models.ViewModels;

namespace TAJAN_TRAVEL.Controllers
{
    // Site-wide search: tours, Nile cruises, destinations, stories, events and FAQs.
    // GET /Search?q=luxor
    public class SearchController : Controller
    {
        private const int MaxPerSection = 12;
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var query = (q ?? string.Empty).Trim();
            if (query.Length > 100) query = query[..100];

            var vm = new SearchViewModel { Query = query };
            ViewData["Title"] = string.IsNullOrEmpty(query) ? "Search" : $"Search: {query}";

            // Every word must match somewhere in the item (so "luxor nile" narrows the results).
            var terms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                             .Distinct(StringComparer.OrdinalIgnoreCase)
                             .Take(5)
                             .ToArray();

            if (terms.Length == 0) return View(vm);

            // ---- Tours + cruises ----
            IQueryable<Tour> tours = _context.Tours
                .Include(t => t.Destination)
                .Where(t => t.IsActive);
            foreach (var term in terms)
            {
                var t1 = term;
                tours = tours.Where(t =>
                    t.TitleEn.Contains(t1) || t.SummaryEn.Contains(t1) || t.DescriptionEn.Contains(t1) ||
                    (t.Destination != null && t.Destination.NameEn.Contains(t1)) ||
                    (t.ActivityType != null && t.ActivityType.NameEn.Contains(t1)));
            }
            var tourHits = await tours
                .OrderByDescending(t => t.IsFeatured).ThenBy(t => t.TitleEn)
                .Take(MaxPerSection * 2)
                .ToListAsync();
            vm.Cruises = tourHits.Where(t => t.Type == TourType.NileCruise).Take(MaxPerSection).ToList();
            vm.Tours = tourHits.Where(t => t.Type != TourType.NileCruise).Take(MaxPerSection).ToList();

            // ---- Destinations ----
            IQueryable<Destination> dests = _context.Destinations.Where(d => d.IsActive);
            foreach (var term in terms)
            {
                var t1 = term;
                dests = dests.Where(d =>
                    d.NameEn.Contains(t1) || d.DescriptionEn.Contains(t1) || d.LongDescriptionEn.Contains(t1) ||
                    d.Category.Contains(t1) || (d.KnownFor != null && d.KnownFor.Contains(t1)));
            }
            vm.Destinations = await dests.OrderBy(d => d.DisplayOrder).Take(MaxPerSection).ToListAsync();

            // ---- Stories ----
            IQueryable<BlogPost> posts = _context.BlogPosts.Include(b => b.Category).Where(b => b.IsPublished);
            foreach (var term in terms)
            {
                var t1 = term;
                posts = posts.Where(b => b.TitleEn.Contains(t1) || b.SummaryEn.Contains(t1) || b.ContentEn.Contains(t1));
            }
            vm.Stories = await posts.OrderByDescending(b => b.PublishedAt).Take(MaxPerSection).ToListAsync();

            // ---- Events ----
            IQueryable<TravelEvent> events = _context.TravelEvents.Include(e => e.Category).Where(e => e.IsActive);
            foreach (var term in terms)
            {
                var t1 = term;
                events = events.Where(e =>
                    e.TitleEn.Contains(t1) || e.SummaryEn.Contains(t1) || e.DescriptionEn.Contains(t1) ||
                    (e.Location != null && e.Location.Contains(t1)));
            }
            vm.Events = await events.OrderBy(e => e.DisplayOrder).Take(MaxPerSection).ToListAsync();

            // ---- FAQs ----
            IQueryable<FAQ> faqs = _context.FAQs.Where(f => f.IsActive);
            foreach (var term in terms)
            {
                var t1 = term;
                faqs = faqs.Where(f => f.QuestionEn.Contains(t1) || f.AnswerEn.Contains(t1));
            }
            vm.Faqs = await faqs.OrderBy(f => f.DisplayOrder).Take(MaxPerSection).ToListAsync();

            return View(vm);
        }
    }
}
