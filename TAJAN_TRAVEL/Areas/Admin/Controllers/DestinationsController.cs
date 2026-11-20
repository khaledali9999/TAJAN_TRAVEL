using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Areas.Admin.Models;
using TAJAN_TRAVEL.Areas.Admin.Services;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Areas.Admin.Controllers
{
    public class DestinationsController : AdminBaseController
    {
        private readonly IWebHostEnvironment _env;

        public DestinationsController(ApplicationDbContext db, IWebHostEnvironment env) : base(db)
        {
            _env = env;
        }

        // ------------------------------------------------------------ LIST
        public async Task<IActionResult> Index()
        {
            var list = await Db.Destinations.AsNoTracking()
                .OrderBy(d => d.DisplayOrder).ThenBy(d => d.NameEn)
                .Select(d => new DestinationRowVm
                {
                    Id = d.Id,
                    NameEn = d.NameEn,
                    NameFr = d.NameFr,
                    Category = d.Category,
                    CoverImageUrl = d.CoverImageUrl,
                    ToursCount = d.Tours.Count,
                    IsFeatured = d.IsFeatured,
                    IsActive = d.IsActive
                })
                .ToListAsync();

            return View(list);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id, string field)
        {
            var d = await Db.Destinations.FindAsync(id);
            if (d == null) return NotFound();

            switch (field)
            {
                case "active": d.IsActive = !d.IsActive; break;
                case "featured": d.IsFeatured = !d.IsFeatured; break;
                default: return BadRequest();
            }

            d.UpdatedAt = DateTime.UtcNow;
            await Db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------------------------- CREATE
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new DestinationFormVm
            {
                DisplayOrder = (await Db.Destinations.MaxAsync(d => (int?)d.DisplayOrder) ?? 0) + 1
            };
            await FillLookups(vm);
            return View("Form", vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public Task<IActionResult> Create(DestinationFormVm vm) => SaveAsync(vm, null);

        // ------------------------------------------------------------ EDIT
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var d = await Db.Destinations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (d == null) return NotFound();

            var vm = new DestinationFormVm
            {
                Id = d.Id,
                NameEn = d.NameEn,
                NameFr = d.NameFr,
                Slug = d.Slug,
                DescriptionEn = d.DescriptionEn,
                DescriptionFr = d.DescriptionFr,
                LongDescriptionEn = d.LongDescriptionEn,
                LongDescriptionFr = d.LongDescriptionFr,
                CoverImageUrl = d.CoverImageUrl,
                HeroImageUrl = d.HeroImageUrl,
                BestTime = d.BestTime,
                BestTimeFr = d.BestTimeFr,
                KnownFor = d.KnownFor,
                KnownForFr = d.KnownForFr,
                CategoryFr = d.CategoryFr,
                Category = d.Category,
                DisplayOrder = d.DisplayOrder,
                IsFeatured = d.IsFeatured,
                IsActive = d.IsActive
            };
            await FillLookups(vm);
            return View("Form", vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DestinationFormVm vm)
        {
            var d = await Db.Destinations.FirstOrDefaultAsync(x => x.Id == id);
            if (d == null) return NotFound();

            vm.Id = id;
            return await SaveAsync(vm, d);
        }

        // ---------------------------------------------------------- DELETE
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var d = await Db.Destinations.Include(x => x.Tours).FirstOrDefaultAsync(x => x.Id == id);
            if (d == null) return NotFound();

            // Tours -> Destination is "Restrict" in the database, so block the delete with a clear message.
            if (d.Tours.Count > 0)
            {
                FlashError($"“{d.NameEn}” still has {d.Tours.Count} tour(s). Move or delete those tours first, " +
                           "or switch the destination off instead.");
                return RedirectToAction(nameof(Index));
            }

            var cover = d.CoverImageUrl;
            var hero = d.HeroImageUrl;
            var name = d.NameEn;

            Db.Destinations.Remove(d);   // wishlist rows pointing here are cascade-deleted
            await Db.SaveChangesAsync();
            AdminImages.DeleteIfUploaded(cover, _env);
            AdminImages.DeleteIfUploaded(hero, _env);

            Flash($"Destination “{name}” deleted.");
            return RedirectToAction(nameof(Index));
        }

        // ------------------------------------------------------------ SAVE
        private async Task<IActionResult> SaveAsync(DestinationFormVm vm, Destination? dest)
        {
            var id = dest?.Id ?? 0;
            var isNew = dest == null;

            AdminImages.ValidateUrl(ModelState, nameof(vm.CoverImageUrl), vm.CoverImageUrl);
            await AdminImages.ValidateFileAsync(ModelState, nameof(vm.CoverImageUrl), vm.CoverFile);
            AdminImages.ValidateUrl(ModelState, nameof(vm.HeroImageUrl), vm.HeroImageUrl);
            await AdminImages.ValidateFileAsync(ModelState, nameof(vm.HeroImageUrl), vm.HeroFile);

            string slug;
            var typed = vm.Slug?.Trim();
            if (!string.IsNullOrEmpty(typed))
            {
                slug = typed;
                if (await Db.Destinations.AnyAsync(x => x.Slug == typed && x.Id != id))
                    ModelState.AddModelError(nameof(vm.Slug), "This slug is already used by another destination.");
            }
            else if (dest != null && !string.IsNullOrEmpty(dest.Slug))
            {
                slug = dest.Slug;
            }
            else
            {
                slug = await Slugger.UniqueAsync(Slugger.Slugify(vm.NameEn), s => Db.Destinations.AnyAsync(x => x.Slug == s));
            }

            if (!ModelState.IsValid)
            {
                await FillLookups(vm);
                return View("Form", vm);
            }

            string? savedUrl = null;
            string? savedHeroUrl = null;
            var oldCover = dest?.CoverImageUrl;
            var oldHero = dest?.HeroImageUrl;

            try
            {
                var cover = await AdminImages.ResolveAsync(vm.CoverImageUrl, vm.CoverFile, _env, "destinations");
                if (vm.CoverFile != null && vm.CoverFile.Length > 0) savedUrl = cover;

                var hero = await AdminImages.ResolveAsync(vm.HeroImageUrl, vm.HeroFile, _env, "destinations");
                if (vm.HeroFile != null && vm.HeroFile.Length > 0) savedHeroUrl = hero;

                dest ??= new Destination { CreatedAt = DateTime.UtcNow };

                dest.NameEn = vm.NameEn.Trim();
                dest.NameFr = vm.NameFr.Trim();
                dest.Slug = slug;
                dest.DescriptionEn = vm.DescriptionEn.Trim();
                dest.DescriptionFr = vm.DescriptionFr.Trim();
                dest.LongDescriptionEn = vm.LongDescriptionEn.OrEmpty();
                dest.LongDescriptionFr = vm.LongDescriptionFr.OrEmpty();
                dest.CoverImageUrl = cover;
                dest.HeroImageUrl = hero;
                dest.BestTime = vm.BestTime.Clean();
                dest.BestTimeFr = vm.BestTimeFr.Clean();
                dest.KnownFor = vm.KnownFor.Clean();
                dest.KnownForFr = vm.KnownForFr.Clean();
                dest.Category = vm.Category.Trim();
                dest.CategoryFr = vm.CategoryFr.Clean();
                dest.DisplayOrder = vm.DisplayOrder;
                dest.IsFeatured = vm.IsFeatured;
                dest.IsActive = vm.IsActive;

                if (isNew) Db.Destinations.Add(dest);
                else dest.UpdatedAt = DateTime.UtcNow;

                await Db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                AdminImages.DeleteIfUploaded(savedUrl, _env);
                AdminImages.DeleteIfUploaded(savedHeroUrl, _env);
                ModelState.AddModelError(string.Empty, "The destination could not be saved. The slug may already exist - try another one.");
                await FillLookups(vm);
                return View("Form", vm);
            }

            if (!isNew && oldCover != dest.CoverImageUrl) AdminImages.DeleteIfUploaded(oldCover, _env);
            if (!isNew && oldHero != dest.HeroImageUrl) AdminImages.DeleteIfUploaded(oldHero, _env);

            Flash(isNew ? $"Destination “{dest.NameEn}” created." : $"Destination “{dest.NameEn}” updated.");
            return RedirectToAction(nameof(Index));
        }

        private async Task FillLookups(DestinationFormVm vm)
        {
            vm.CategorySuggestions = await Db.Destinations.AsNoTracking()
                .Where(d => d.Category != "")
                .Select(d => d.Category).Distinct().OrderBy(c => c)
                .ToListAsync();
        }
    }
}
