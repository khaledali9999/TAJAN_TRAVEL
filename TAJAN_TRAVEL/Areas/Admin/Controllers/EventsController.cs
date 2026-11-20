using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Areas.Admin.Models;
using TAJAN_TRAVEL.Areas.Admin.Services;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Areas.Admin.Controllers
{
    public class EventsController : AdminBaseController
    {
        private readonly IWebHostEnvironment _env;

        public EventsController(ApplicationDbContext db, IWebHostEnvironment env) : base(db)
        {
            _env = env;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var query = Db.TravelEvents.AsNoTracking().Include(e => e.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var t = q.Trim();
                query = query.Where(e => e.TitleEn.Contains(t) || e.TitleFr.Contains(t) || (e.Location != null && e.Location.Contains(t)));
            }

            ViewData["Q"] = q;
            return View(await query.OrderBy(e => e.DisplayOrder).ThenBy(e => e.EventDate).ToListAsync());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id, string field, string? q)
        {
            var e = await Db.TravelEvents.FindAsync(id);
            if (e == null) return NotFound();

            switch (field)
            {
                case "active": e.IsActive = !e.IsActive; break;
                case "featured": e.IsFeatured = !e.IsFeatured; break;
                default: return BadRequest();
            }

            await Db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { q });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new EventFormVm
            {
                DisplayOrder = (await Db.TravelEvents.MaxAsync(e => (int?)e.DisplayOrder) ?? 0) + 1
            };
            await FillLookups(vm);
            return View("Form", vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public Task<IActionResult> Create(EventFormVm vm) => SaveAsync(vm, null);

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var e = await Db.TravelEvents.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return NotFound();

            var vm = new EventFormVm
            {
                Id = e.Id,
                TitleEn = e.TitleEn,
                TitleFr = e.TitleFr,
                Slug = e.Slug,
                SummaryEn = e.SummaryEn,
                SummaryFr = e.SummaryFr,
                DescriptionEn = e.DescriptionEn,
                DescriptionFr = e.DescriptionFr,
                CoverImageUrl = e.CoverImageUrl,
                EventCategoryId = e.EventCategoryId,
                Location = e.Location,
                LocationFr = e.LocationFr,
                EventDate = e.EventDate,
                DateLabel = e.DateLabel,
                ReadTimeMinutes = e.ReadTimeMinutes,
                DisplayOrder = e.DisplayOrder,
                IsFeatured = e.IsFeatured,
                IsActive = e.IsActive
            };
            await FillLookups(vm);
            return View("Form", vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventFormVm vm)
        {
            var e = await Db.TravelEvents.FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return NotFound();

            vm.Id = id;
            return await SaveAsync(vm, e);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var e = await Db.TravelEvents.FindAsync(id);
            if (e == null) return NotFound();

            var cover = e.CoverImageUrl;
            var title = e.TitleEn;

            Db.TravelEvents.Remove(e);
            await Db.SaveChangesAsync();
            AdminImages.DeleteIfUploaded(cover, _env);

            Flash($"Event “{title}” deleted.");
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> SaveAsync(EventFormVm vm, TravelEvent? ev)
        {
            var id = ev?.Id ?? 0;
            var isNew = ev == null;

            if (vm.EventCategoryId.HasValue && !await Db.EventCategories.AnyAsync(c => c.Id == vm.EventCategoryId.Value))
                ModelState.AddModelError(nameof(vm.EventCategoryId), "This category no longer exists.");

            if (vm.EventDate == null && string.IsNullOrWhiteSpace(vm.DateLabel))
                ModelState.AddModelError(nameof(vm.DateLabel), "Pick a date, or write a label such as “On request”.");

            AdminImages.ValidateUrl(ModelState, nameof(vm.CoverImageUrl), vm.CoverImageUrl);
            await AdminImages.ValidateFileAsync(ModelState, nameof(vm.CoverImageUrl), vm.CoverFile);

            string slug;
            var typed = vm.Slug?.Trim();
            if (!string.IsNullOrEmpty(typed))
            {
                slug = typed;
                if (await Db.TravelEvents.AnyAsync(x => x.Slug == typed && x.Id != id))
                    ModelState.AddModelError(nameof(vm.Slug), "This slug is already used by another event.");
            }
            else if (ev != null && !string.IsNullOrEmpty(ev.Slug))
            {
                slug = ev.Slug;
            }
            else
            {
                slug = await Slugger.UniqueAsync(Slugger.Slugify(vm.TitleEn), s => Db.TravelEvents.AnyAsync(x => x.Slug == s));
            }

            if (!ModelState.IsValid)
            {
                await FillLookups(vm);
                return View("Form", vm);
            }

            string? savedUrl = null;
            var oldCover = ev?.CoverImageUrl;

            try
            {
                var cover = await AdminImages.ResolveAsync(vm.CoverImageUrl, vm.CoverFile, _env, "events");
                if (vm.CoverFile != null && vm.CoverFile.Length > 0) savedUrl = cover;

                ev ??= new TravelEvent { CreatedAt = DateTime.UtcNow };

                ev.TitleEn = vm.TitleEn.Trim();
                ev.TitleFr = vm.TitleFr.Trim();
                ev.Slug = slug;
                ev.SummaryEn = vm.SummaryEn.Trim();
                ev.SummaryFr = vm.SummaryFr.Trim();
                ev.DescriptionEn = vm.DescriptionEn.Trim();
                ev.DescriptionFr = vm.DescriptionFr.Trim();
                ev.CoverImageUrl = cover;
                ev.EventCategoryId = vm.EventCategoryId;
                ev.Location = vm.Location.Clean();
                ev.LocationFr = vm.LocationFr.Clean();
                ev.EventDate = vm.EventDate;
                ev.DateLabel = vm.DateLabel.Clean();
                ev.ReadTimeMinutes = vm.ReadTimeMinutes;
                ev.DisplayOrder = vm.DisplayOrder;
                ev.IsFeatured = vm.IsFeatured;
                ev.IsActive = vm.IsActive;

                if (isNew) Db.TravelEvents.Add(ev);
                await Db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                AdminImages.DeleteIfUploaded(savedUrl, _env);
                ModelState.AddModelError(string.Empty, "The event could not be saved. The slug may already exist - try another one.");
                await FillLookups(vm);
                return View("Form", vm);
            }

            if (!isNew && oldCover != ev.CoverImageUrl) AdminImages.DeleteIfUploaded(oldCover, _env);

            Flash(isNew ? $"Event “{ev.TitleEn}” created." : $"Event “{ev.TitleEn}” updated.");
            return RedirectToAction(nameof(Index));
        }

        private async Task FillLookups(EventFormVm vm)
        {
            var cats = await Db.EventCategories.AsNoTracking()
                .OrderBy(c => c.DisplayOrder).ThenBy(c => c.NameEn)
                .Select(c => new { c.Id, c.NameEn }).ToListAsync();
            vm.CategoryOptions = cats.Select(c => new SelectListItem(c.NameEn, c.Id.ToString())).ToList();
        }
    }
}
