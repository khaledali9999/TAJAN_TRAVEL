using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Areas.Admin.Models;
using TAJAN_TRAVEL.Areas.Admin.Services;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Areas.Admin.Controllers
{
    // One screen for the three small "lookup" lists:
    //   blog   = story categories (BlogCategory)
    //   event  = event categories (EventCategory)
    //   activity = tour activity types (ActivityType)
    public class CategoriesController : AdminBaseController
    {
        public CategoriesController(ApplicationDbContext db) : base(db) { }

        // The three lists now live on their own pages (Stories / Events / Tours).
        // This URL is kept only so old bookmarks still work.
        public IActionResult Index() => RedirectToAction("Index", "Stories");

        // Where to go after Save / Delete: back to the page the form was on.
        private IActionResult Back(string? returnTo) =>
            returnTo is "Stories" or "Events" or "Tours"
                ? RedirectToAction("Index", returnTo)
                : RedirectToAction("Index", "Stories");

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string kind, LookupRowVm row, string? returnTo)
        {
            if (!ModelState.IsValid)
            {
                var first = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault(m => !string.IsNullOrEmpty(m));
                FlashError(first ?? "Please check the values and try again.");
                return Back(returnTo);
            }

            var nameEn = row.NameEn!.Trim();
            var nameFr = row.NameFr!.Trim();
            var isNew = row.Id == 0;

            switch (kind)
            {
                case "blog":
                    {
                        var c = isNew ? new BlogCategory() : await Db.BlogCategories.FindAsync(row.Id);
                        if (c == null) return NotFound();

                        var slug = Slugger.Slugify(row.Slug.Clean() ?? nameEn);
                        if (string.IsNullOrEmpty(slug)) slug = "category";
                        if (await Db.BlogCategories.AnyAsync(x => x.Slug == slug && x.Id != row.Id))
                        {
                            FlashError($"The slug “{slug}” is already used by another story category.");
                            return Back(returnTo);
                        }

                        c.NameEn = nameEn; c.NameFr = nameFr; c.Slug = slug;
                        c.DisplayOrder = row.DisplayOrder; c.ShowInFilter = row.ShowInFilter;
                        if (isNew) Db.BlogCategories.Add(c);
                        break;
                    }
                case "event":
                    {
                        var c = isNew ? new EventCategory() : await Db.EventCategories.FindAsync(row.Id);
                        if (c == null) return NotFound();

                        var slug = Slugger.Slugify(row.Slug.Clean() ?? nameEn);
                        if (string.IsNullOrEmpty(slug)) slug = "category";
                        if (await Db.EventCategories.AnyAsync(x => x.Slug == slug && x.Id != row.Id))
                        {
                            FlashError($"The slug “{slug}” is already used by another event category.");
                            return Back(returnTo);
                        }

                        c.NameEn = nameEn; c.NameFr = nameFr; c.Slug = slug; c.DisplayOrder = row.DisplayOrder;
                        if (isNew) Db.EventCategories.Add(c);
                        break;
                    }
                case "activity":
                    {
                        var a = isNew ? new ActivityType() : await Db.ActivityTypes.FindAsync(row.Id);
                        if (a == null) return NotFound();

                        a.NameEn = nameEn; a.NameFr = nameFr; a.IconClass = row.IconClass.Clean();
                        if (isNew) Db.ActivityTypes.Add(a);
                        break;
                    }
                default:
                    return BadRequest();
            }

            await Db.SaveChangesAsync();
            Flash(isNew ? $"“{nameEn}” added." : $"“{nameEn}” saved.");
            return Back(returnTo);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string kind, int id, string? returnTo)
        {
            // Posts / events / tours that used the item are kept and simply become "uncategorised" (FK = set null).
            switch (kind)
            {
                case "blog":
                    {
                        var c = await Db.BlogCategories.FindAsync(id);
                        if (c == null) return NotFound();
                        Db.BlogCategories.Remove(c);
                        break;
                    }
                case "event":
                    {
                        var c = await Db.EventCategories.FindAsync(id);
                        if (c == null) return NotFound();
                        Db.EventCategories.Remove(c);
                        break;
                    }
                case "activity":
                    {
                        var a = await Db.ActivityTypes.FindAsync(id);
                        if (a == null) return NotFound();
                        Db.ActivityTypes.Remove(a);
                        break;
                    }
                default:
                    return BadRequest();
            }

            await Db.SaveChangesAsync();
            Flash("Deleted.");
            return Back(returnTo);
        }
    }
}
