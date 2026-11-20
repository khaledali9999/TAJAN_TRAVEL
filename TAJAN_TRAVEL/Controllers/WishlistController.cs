using TAJAN_TRAVEL.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.Entities;
using TAJAN_TRAVEL.Models.ViewModels;

namespace TAJAN_TRAVEL.Controllers
{
    // Backs the Wishlist popup (Views/Shared/_WishlistModal.cshtml).
    // No login on this site, so visitors are identified by an anonymous,
    // HttpOnly session cookie instead of a user account — every endpoint
    // here reads/writes only the rows that belong to that cookie.
    public class WishlistController : Controller
    {
        private const string SessionCookieName = "tajan_wsid";
        private readonly ApplicationDbContext _context;

        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /Wishlist
        // Full current wishlist for this visitor - called once when the popup opens
        // (and on page load, to mark the right hearts as active).
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var sessionId = GetOrCreateSessionId();
            return Json(await BuildResult(sessionId));
        }

        // POST /Wishlist/Toggle   body: { type: 0|1 ("Tour"|"Destination"), refId: 5 }
        // Adds the item if it isn't saved yet, removes it if it already is.
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Toggle([FromBody] WishlistToggleRequest? request)
        {
            if (request == null || request.RefId <= 0)
                return BadRequest();

            var sessionId = GetOrCreateSessionId();

            var existing = await _context.WishlistItems.FirstOrDefaultAsync(w =>
                w.SessionId == sessionId &&
                w.ItemType == request.Type &&
                (request.Type == WishlistItemType.Tour ? w.TourId == request.RefId : w.DestinationId == request.RefId));

            if (existing != null)
            {
                _context.WishlistItems.Remove(existing);
            }
            else
            {
                var targetExists = request.Type == WishlistItemType.Tour
                    ? await _context.Tours.AnyAsync(t => t.Id == request.RefId && t.IsActive)
                    : await _context.Destinations.AnyAsync(d => d.Id == request.RefId && d.IsActive);

                if (!targetExists)
                    return NotFound();

                _context.WishlistItems.Add(new WishlistItem
                {
                    SessionId = sessionId,
                    ItemType = request.Type,
                    TourId = request.Type == WishlistItemType.Tour ? request.RefId : null,
                    DestinationId = request.Type == WishlistItemType.Destination ? request.RefId : null
                });
            }

            await _context.SaveChangesAsync();
            return Json(await BuildResult(sessionId));
        }

        // POST /Wishlist/Remove/5   (5 = WishlistItem.Id, from the popup's own remove button)
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var sessionId = GetOrCreateSessionId();

            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.Id == id && w.SessionId == sessionId);

            if (item != null)
            {
                _context.WishlistItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return Json(await BuildResult(sessionId));
        }

        private async Task<WishlistResultViewModel> BuildResult(string sessionId)
        {
            var items = await _context.WishlistItems
                .Where(w => w.SessionId == sessionId)
                .Include(w => w.Tour).ThenInclude(t => t!.Destination)
                .Include(w => w.Destination)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

            var dtos = new List<WishlistItemViewModel>();
            var orphansRemoved = false;

            foreach (var item in items)
            {
                var dto = ToViewModel(item);
                if (dto != null)
                    dtos.Add(dto);
                else
                    orphansRemoved = true;
            }

            if (orphansRemoved)
                await _context.SaveChangesAsync();

            return new WishlistResultViewModel { Count = dtos.Count, Items = dtos };
        }

        private WishlistItemViewModel? ToViewModel(WishlistItem w)
        {
            if (w.ItemType == WishlistItemType.Tour && w.Tour != null)
            {
                var tour = w.Tour;
                var durationLabel = tour.DurationDays <= 1 ? Lang.T("8 Hours") : $"{tour.DurationDays} {Lang.T("Days")}";

                return new WishlistItemViewModel
                {
                    Id = w.Id,
                    Key = $"tour-{tour.Id}",
                    Badge = tour.IsPrivate ? Lang.T("Private Day Tour") : Lang.TourTypeLabel(tour.Type),
                    Title = tour.Title,
                    Meta = $"{durationLabel} · {tour.Destination?.Name}",
                    Price = $"{Lang.T("From")} {SiteCurrency.Format(tour.PriceFrom, tour.Currency, "N0")}",
                    Image = tour.CoverImageUrl ?? "",
                    Url = Url.Action("Details", "Tours", new { slug = tour.Slug }) ?? "#",
                    LinkLabel = Lang.T("View Tour")
                };
            }

            if (w.ItemType == WishlistItemType.Destination && w.Destination != null)
            {
                var destination = w.Destination;

                return new WishlistItemViewModel
                {
                    Id = w.Id,
                    Key = $"dest-{destination.Id}",
                    Badge = Lang.T("Destination"),
                    Title = destination.Name,
                    Meta = Lang.T(Lang.Pick(destination.Category, destination.CategoryFr)),
                    Price = destination.Description,
                    Image = destination.CoverImageUrl ?? "",
                    Url = Url.Action("Details", "Destinations", new { slug = destination.Slug }) ?? "#",
                    LinkLabel = Lang.T("Explore Destination")
                };
            }

            // Underlying Tour/Destination was deleted after being saved - drop the orphaned row.
            _context.WishlistItems.Remove(w);
            return null;
        }

        private string GetOrCreateSessionId()
        {
            if (Request.Cookies.TryGetValue(SessionCookieName, out var existing) && !string.IsNullOrWhiteSpace(existing))
                return existing;

            var newId = Guid.NewGuid().ToString("N");
            Response.Cookies.Append(SessionCookieName, newId, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(180)
            });
            return newId;
        }
    }
}
