using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Data;

namespace TAJAN_TRAVEL.Areas.Admin.Controllers
{
    public class NotificationsController : AdminBaseController
    {
        public NotificationsController(ApplicationDbContext db) : base(db) { }

        public async Task<IActionResult> Index()
        {
            var list = await Db.ContactMessages.AsNoTracking()
                .OrderBy(m => m.IsRead)
                .ThenByDescending(m => m.CreatedAt)
                .Take(100)
                .ToListAsync();

            return View(list);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkRead(int id)
        {
            var m = await Db.ContactMessages.FindAsync(id);
            if (m == null) return NotFound();

            m.IsRead = true;
            await Db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllRead()
        {
            var unread = await Db.ContactMessages.Where(m => !m.IsRead).ToListAsync();
            foreach (var m in unread) m.IsRead = true;
            await Db.SaveChangesAsync();

            TempData["Flash"] = "All messages marked as read.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkUnread(int id)
        {
            var m = await Db.ContactMessages.FindAsync(id);
            if (m == null) return NotFound();

            m.IsRead = false;
            await Db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var m = await Db.ContactMessages.FindAsync(id);
            if (m == null) return NotFound();

            Db.ContactMessages.Remove(m);
            await Db.SaveChangesAsync();

            FlashError("Message deleted.");
            return RedirectToAction(nameof(Index));
        }
    }
}
