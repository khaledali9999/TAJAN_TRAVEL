using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.ViewModels;

namespace TAJAN_TRAVEL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel
            {
                FeaturedTours = await _context.Tours
                    .Include(t => t.Destination)
                    .Where(t => t.IsActive && t.IsFeatured)
                    .OrderByDescending(t => t.CreatedAt)
                    .Take(6)
                    .ToListAsync(),

                Testimonials = await _context.Testimonials
                    .Where(t => t.IsApproved)
                    .OrderBy(t => t.DisplayOrder).ThenBy(t => t.Id)
                    .ToListAsync(),

                FeaturedDestinations = await _context.Destinations
                    .Where(d => d.IsActive && d.IsFeatured)
                    .OrderBy(d => d.DisplayOrder)
                    .Take(4)
                    .ToListAsync(),

                LatestBlogPosts = await _context.BlogPosts
    .Include(b => b.Category)
    .Where(b => b.IsPublished)
    .OrderByDescending(b => b.PublishedAt)
    .Take(3)
    .ToListAsync(),

                Faqs = await _context.FAQs
    .Where(f => f.IsActive)
    .OrderBy(f => f.DisplayOrder)
    .Take(6)
    .ToListAsync(),

                LatestEvents = await _context.TravelEvents
                    .Include(e => e.Category)
                    .Where(e => e.IsActive)
                    .OrderBy(e => e.DisplayOrder).ThenBy(e => e.EventDate)
                    .Take(3)
                    .ToListAsync(),

                SearchDestinations = await _context.Destinations
                    .Where(d => d.IsActive)
                    .OrderBy(d => d.DisplayOrder)
                    .ToListAsync()

            };

            return View(vm);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}