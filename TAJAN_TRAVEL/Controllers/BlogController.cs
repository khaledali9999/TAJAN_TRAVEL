using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.ViewModels;

namespace TAJAN_TRAVEL.Controllers
{
    public class BlogController : Controller
    {
        private const int PageSize = 6;
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /Blog
        // GET /Blog?category=cairo&page=2
        // GET /Blog?story=beyond-the-pyramids   (opens that story's popup)
        public async Task<IActionResult> Index(string? category, int page = 1, string? story = null)
        {
            if (page < 1) page = 1;

            var published = _context.BlogPosts
                .Include(b => b.Category)
                .Where(b => b.IsPublished);

            var isFiltered = !string.IsNullOrWhiteSpace(category);

            // The big "Featured Story" is only shown on the unfiltered list
            Models.Entities.BlogPost? featured = null;
            if (!isFiltered)
            {
                featured = await published
                    .Where(b => b.IsFeatured)
                    .OrderByDescending(b => b.PublishedAt)
                    .FirstOrDefaultAsync();
            }

            var query = published;
            if (featured != null)
                query = query.Where(b => b.Id != featured.Id);

            if (isFiltered)
                query = query.Where(b => b.Category != null && b.Category.Slug == category);

            query = query.OrderByDescending(b => b.PublishedAt).ThenBy(b => b.Id);

            var total = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
            page = Math.Min(page, totalPages);

            var vm = new StoriesViewModel
            {
                Featured = featured,
                Posts = await query.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync(),
                Categories = await _context.BlogCategories
                    .Where(c => c.ShowInFilter && c.Posts.Any(p => p.IsPublished))
                    .OrderBy(c => c.DisplayOrder).ThenBy(c => c.NameEn)
                    .ToListAsync(),
                Category = category,
                CurrentPage = page,
                TotalPages = totalPages
            };

            // Filter chips / pagination fetch this URL via JS: return only the filterable block
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                Response.Headers["Vary"] = "X-Requested-With";
                return PartialView("_StoriesResults", vm);
            }

            vm.Testimonials = await _context.Testimonials
                .Where(t => t.IsApproved)
                .OrderBy(t => t.DisplayOrder).ThenBy(t => t.Id)
                .ToListAsync();

            vm.Moments = await published
                .Where(b => b.CoverImageUrl != null && b.CoverImageUrl != "")
                .OrderByDescending(b => b.PublishedAt)
                .Take(6)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(story))
            {
                vm.DeepLinkStory = await published.FirstOrDefaultAsync(b => b.Slug == story);
            }

            return View(vm);
        }

        // GET /blog/beyond-the-pyramids  (the Home page links here) -> opens the popup on /Blog
        [Route("blog/{slug}")]
        public IActionResult Details(string slug)
        {
            return RedirectToAction(nameof(Index), new { story = slug });
        }
    }
}