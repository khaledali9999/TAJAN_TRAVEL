using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Areas.Admin.Models;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Areas.Admin.Controllers
{
    public class TourRequestsController : AdminBaseController
    {
        private const int PageSize = 15;

        public TourRequestsController(ApplicationDbContext db) : base(db) { }

        public async Task<IActionResult> Index(string? status, string? type, string? q, int page = 1)
        {
            var query = Db.Inquiries.AsNoTracking().Include(i => i.Tour).AsQueryable();

            if (Enum.TryParse<InquiryStatus>(status, true, out var st) && Enum.IsDefined(st))
                query = query.Where(i => i.Status == st);
            else
                status = null;

            InquiryType? typeFilter = null;
            if (Enum.TryParse<InquiryType>(type, true, out var tp) && Enum.IsDefined(tp))
            {
                typeFilter = tp;
                query = query.Where(i => i.Type == tp);
            }
            else
                type = null;

            if (!string.IsNullOrWhiteSpace(q))
            {
                var t = q.Trim();
                query = query.Where(i =>
                    i.FullName.Contains(t) ||
                    i.Email.Contains(t) ||
                    (i.Tour != null && i.Tour.TitleEn.Contains(t)));
            }

            var total = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
            page = Math.Clamp(page, 1, totalPages);

            var items = await query
                .OrderByDescending(i => i.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var typeCounts = await Db.Inquiries
                .GroupBy(i => i.Type)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count);

            // status pills count only the requests of the selected source
            var countsQuery = Db.Inquiries.AsQueryable();
            if (typeFilter.HasValue) countsQuery = countsQuery.Where(i => i.Type == typeFilter.Value);

            var counts = await countsQuery
                .GroupBy(i => i.Status)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count);

            return View(new TourRequestsIndexViewModel
            {
                Items = items,
                Status = status,
                Type = type,
                Q = q,
                Page = page,
                TotalPages = totalPages,
                Total = total,
                Counts = counts,
                TypeCounts = typeCounts
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            var inquiry = await Db.Inquiries
                .Include(i => i.Tour).ThenInclude(t => t!.Destination)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inquiry == null) return NotFound();
            return View(inquiry);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, InquiryStatus status)
        {
            if (!Enum.IsDefined(status)) return BadRequest();

            var inquiry = await Db.Inquiries.FindAsync(id);
            if (inquiry == null) return NotFound();

            inquiry.Status = status;
            await Db.SaveChangesAsync();

            TempData["Flash"] = $"{AdminFormat.Rq(id)} marked as {AdminFormat.Pretty(status.ToString())}.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var inquiry = await Db.Inquiries.FindAsync(id);
            if (inquiry == null) return NotFound();

            Db.Inquiries.Remove(inquiry);
            await Db.SaveChangesAsync();

            FlashError($"{AdminFormat.Rq(id)} was permanently deleted.");
            return RedirectToAction(nameof(Index));
        }
    }
}
