using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Areas.Admin.Models;
using TAJAN_TRAVEL.Data;

namespace TAJAN_TRAVEL.Areas.Admin.Components
{
    public record LookupSectionVm(string Title, string Hint, string Kind, List<LookupRowVm> Rows);

    // The small editable list of categories/types shown at the bottom of a management page.
    //   kind = "blog"     -> story categories  (Stories page)
    //   kind = "event"    -> event categories  (Events page)
    //   kind = "activity" -> tour activity types (Tours page)
    public class LookupSectionViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public LookupSectionViewComponent(ApplicationDbContext db) { _db = db; }

        public async Task<IViewComponentResult> InvokeAsync(string kind, string returnTo)
        {
            ViewData["ReturnTo"] = returnTo;

            LookupSectionVm vm = kind switch
            {
                "blog" => new LookupSectionVm("Story categories",
                    "Used by Stories & Journal. \u201CFilter chip\u201D shows the category as a filter on the Stories page.", kind,
                    await _db.BlogCategories.AsNoTracking()
                        .OrderBy(c => c.DisplayOrder).ThenBy(c => c.NameEn)
                        .Select(c => new LookupRowVm
                        {
                            Id = c.Id, NameEn = c.NameEn, NameFr = c.NameFr, Slug = c.Slug,
                            DisplayOrder = c.DisplayOrder, ShowInFilter = c.ShowInFilter, UsageCount = c.Posts.Count
                        }).ToListAsync()),

                "event" => new LookupSectionVm("Event categories", "Used by Events.", kind,
                    await _db.EventCategories.AsNoTracking()
                        .OrderBy(c => c.DisplayOrder).ThenBy(c => c.NameEn)
                        .Select(c => new LookupRowVm
                        {
                            Id = c.Id, NameEn = c.NameEn, NameFr = c.NameFr, Slug = c.Slug,
                            DisplayOrder = c.DisplayOrder, UsageCount = c.Events.Count
                        }).ToListAsync()),

                "activity" => new LookupSectionVm("Tour activity types", "Used by Tours (e.g. Culture & History, Adventure).", kind,
                    await _db.ActivityTypes.AsNoTracking()
                        .OrderBy(a => a.NameEn)
                        .Select(a => new LookupRowVm
                        {
                            Id = a.Id, NameEn = a.NameEn, NameFr = a.NameFr, IconClass = a.IconClass, UsageCount = a.Tours.Count
                        }).ToListAsync()),

                _ => throw new ArgumentOutOfRangeException(nameof(kind))
            };

            return View(vm);
        }
    }
}
