using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Areas.Admin.Models;
using TAJAN_TRAVEL.Areas.Admin.Services;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Areas.Admin.Controllers
{
    // Key / value site settings (SiteSetting table).
    public class SettingsController : AdminBaseController
    {
        public SettingsController(ApplicationDbContext db) : base(db) { }

        public async Task<IActionResult> Index()
        {
            return View(await Db.SiteSettings.AsNoTracking().Where(s => !s.Key.StartsWith(AdminAccount.Prefix)).OrderBy(s => s.Key).ToListAsync());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(SettingRowVm row)
        {
            if (!ModelState.IsValid)
            {
                var first = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault(m => !string.IsNullOrEmpty(m));
                FlashError(first ?? "Please check the values and try again.");
                return RedirectToAction(nameof(Index));
            }

            if (row.Id == 0)
            {
                var key = row.Key!.Trim();
                if (key.StartsWith(AdminAccount.Prefix, StringComparison.OrdinalIgnoreCase))
                {
                    FlashError("Keys starting with “Admin.” are reserved (use the Settings page of your account).");
                    return RedirectToAction(nameof(Index));
                }
                if (await Db.SiteSettings.AnyAsync(s => s.Key == key))
                {
                    FlashError($"A setting named “{key}” already exists.");
                    return RedirectToAction(nameof(Index));
                }

                Db.SiteSettings.Add(new SiteSetting
                {
                    Key = key,
                    Value = row.Value!.Trim(),
                    Description = row.Description.Clean()
                });
            }
            else
            {
                var s = await Db.SiteSettings.FindAsync(row.Id);
                if (s == null || s.Key.StartsWith(AdminAccount.Prefix, StringComparison.OrdinalIgnoreCase)) return NotFound();

                // the key itself is never renamed - code may depend on it
                s.Value = row.Value!.Trim();
                s.Description = row.Description.Clean();
            }

            await Db.SaveChangesAsync();
            Flash("Setting saved.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await Db.SiteSettings.FindAsync(id);
            if (s == null || s.Key.StartsWith(AdminAccount.Prefix, StringComparison.OrdinalIgnoreCase)) return NotFound();

            Db.SiteSettings.Remove(s);
            await Db.SaveChangesAsync();

            Flash($"Setting “{s.Key}” deleted.");
            return RedirectToAction(nameof(Index));
        }
    }
}
