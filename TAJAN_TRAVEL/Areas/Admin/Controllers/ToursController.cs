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
    public class ToursController : AdminBaseController
    {
        private const int MaxGalleryUploads = 20;
        private const int PriceTiers = 6;   // group sizes 1..6 are shown in the admin form
        private readonly IWebHostEnvironment _env;

        public ToursController(ApplicationDbContext db, IWebHostEnvironment env) : base(db)
        {
            _env = env;
        }

        // ------------------------------------------------------------ LIST
        public async Task<IActionResult> Index(string? q, int? destinationId, string? status)
        {
            var query = Db.Tours.AsNoTracking().Include(t => t.Destination).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var t = q.Trim();
                query = query.Where(x => x.TitleEn.Contains(t) || x.TitleFr.Contains(t));
            }

            if (destinationId.HasValue)
                query = query.Where(x => x.DestinationId == destinationId.Value);

            switch (status)
            {
                case "active": query = query.Where(x => x.IsActive); break;
                case "inactive": query = query.Where(x => !x.IsActive); break;
                case "featured": query = query.Where(x => x.IsFeatured); break;
                default: status = null; break;
            }

            var vm = new ToursIndexVm
            {
                Items = await query.OrderByDescending(t => t.CreatedAt).ToListAsync(),
                Q = q,
                DestinationId = destinationId,
                Status = status,
                Destinations = await DestinationItems()
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id, string field, string? q)
        {
            var tour = await Db.Tours.FindAsync(id);
            if (tour == null) return NotFound();

            switch (field)
            {
                case "active": tour.IsActive = !tour.IsActive; break;
                case "featured": tour.IsFeatured = !tour.IsFeatured; break;
                default: return BadRequest();
            }

            tour.UpdatedAt = DateTime.UtcNow;
            await Db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { q });
        }

        // ---------------------------------------------------------- CREATE
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new TourFormVm();
            vm.Itinerary.Add(new ItineraryRowVm { DayNumber = 1 });
            for (var n = 1; n <= PriceTiers; n++) vm.Prices.Add(new PriceRowVm { Persons = n });
            await FillLookups(vm);
            return View("Form", vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public Task<IActionResult> Create(TourFormVm vm) => SaveAsync(vm, null);

        // ------------------------------------------------------------ EDIT
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tour = await Db.Tours.AsNoTracking()
                .Include(t => t.Images)
                .Include(t => t.Itinerary)
                .Include(t => t.Prices)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tour == null) return NotFound();

            var vm = new TourFormVm
            {
                Id = tour.Id,
                TitleEn = tour.TitleEn,
                TitleFr = tour.TitleFr,
                Slug = tour.Slug,
                SummaryEn = tour.SummaryEn,
                SummaryFr = tour.SummaryFr,
                DescriptionEn = tour.DescriptionEn,
                DescriptionFr = tour.DescriptionFr,
                QuoteEn = tour.QuoteEn,
                QuoteFr = tour.QuoteFr,
                HighlightsEn = tour.HighlightsEn,
                HighlightsFr = tour.HighlightsFr,
                IncludedEn = tour.IncludedEn,
                IncludedFr = tour.IncludedFr,
                ExcludedEn = tour.ExcludedEn,
                ExcludedFr = tour.ExcludedFr,
                ImportantInfoEn = tour.ImportantInfoEn,
                ImportantInfoFr = tour.ImportantInfoFr,
                AvailabilityEn = tour.AvailabilityEn,
                AvailabilityFr = tour.AvailabilityFr,
                PaymentNoteEn = tour.PaymentNoteEn,
                PaymentNoteFr = tour.PaymentNoteFr,
                DurationHours = tour.DurationHours,
                Type = tour.Type,
                DurationDays = tour.DurationDays,
                PriceFrom = tour.PriceFrom,
                Currency = tour.Currency,
                CoverImageUrl = tour.CoverImageUrl,
                IsPrivate = tour.IsPrivate,
                IsFeatured = tour.IsFeatured,
                IsActive = tour.IsActive,
                DestinationId = tour.DestinationId,
                ActivityTypeId = tour.ActivityTypeId,
                Itinerary = tour.Itinerary.OrderBy(i => i.DayNumber).Select(i => new ItineraryRowVm
                {
                    DayNumber = i.DayNumber,
                    TitleEn = i.TitleEn,
                    TitleFr = i.TitleFr,
                    DescriptionEn = i.DescriptionEn,
                    DescriptionFr = i.DescriptionFr
                }).ToList(),
                Gallery = tour.Images.OrderBy(i => i.DisplayOrder).ThenBy(i => i.Id).Select(i => new GalleryRowVm
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    AltTextEn = i.AltTextEn,
                    AltTextFr = i.AltTextFr
                }).ToList()
            };

            for (var n = 1; n <= PriceTiers; n++)
            {
                var existing = tour.Prices.FirstOrDefault(p => p.Persons == n);
                vm.Prices.Add(new PriceRowVm { Persons = n, PricePerPerson = existing?.PricePerPerson });
            }

            await FillLookups(vm);
            return View("Form", vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TourFormVm vm)
        {
            var tour = await Db.Tours
                .Include(t => t.Images)
                .Include(t => t.Itinerary)
                .Include(t => t.Prices)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tour == null) return NotFound();

            vm.Id = id;
            return await SaveAsync(vm, tour);
        }

        // ---------------------------------------------------------- DELETE
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var tour = await Db.Tours.Include(t => t.Images).FirstOrDefaultAsync(t => t.Id == id);
            if (tour == null) return NotFound();

            var files = tour.Images.Select(i => i.ImageUrl).Append(tour.CoverImageUrl).ToList();
            var title = tour.TitleEn;

            // Images / itinerary / wishlist rows are removed by the database (cascade);
            // inquiries and reviews keep existing and simply lose their tour link (set null).
            Db.Tours.Remove(tour);
            await Db.SaveChangesAsync();

            foreach (var f in files) AdminImages.DeleteIfUploaded(f, _env);

            Flash($"Tour “{title}” deleted.");
            return RedirectToAction(nameof(Index));
        }

        // ------------------------------------------------------------ SAVE
        private async Task<IActionResult> SaveAsync(TourFormVm vm, Tour? tour)
        {
            var id = tour?.Id ?? 0;
            var isNew = tour == null;

            vm.Currency = (vm.Currency ?? "USD").Trim().ToUpperInvariant();

            // itinerary rows (indexes must match the posted field names so errors land on the right row)
            for (var i = 0; i < vm.Itinerary.Count; i++)
            {
                var r = vm.Itinerary[i];
                if (r.IsBlank) continue;

                if (r.DayNumber < 1 || r.DayNumber > 60)
                    ModelState.AddModelError($"Itinerary[{i}].DayNumber", "Day must be 1–60.");
                if (string.IsNullOrWhiteSpace(r.TitleEn))
                    ModelState.AddModelError($"Itinerary[{i}].TitleEn", "English title is required.");
                if (string.IsNullOrWhiteSpace(r.TitleFr))
                    ModelState.AddModelError($"Itinerary[{i}].TitleFr", "French title is required.");
                if ((r.TitleEn?.Length ?? 0) > 150 || (r.TitleFr?.Length ?? 0) > 150)
                    ModelState.AddModelError($"Itinerary[{i}].TitleEn", "Titles can be 150 characters at most.");
            }

            // group prices
            for (var i = 0; i < vm.Prices.Count; i++)
            {
                var pr = vm.Prices[i];
                if (pr.PricePerPerson.HasValue && (pr.PricePerPerson.Value <= 0 || pr.PricePerPerson.Value > 1000000))
                    ModelState.AddModelError($"Prices[{i}].PricePerPerson", "Enter a valid price.");
            }
            if (vm.Prices.Select(p => p.Persons).Distinct().Count() != vm.Prices.Count)
                ModelState.AddModelError(string.Empty, "Each group size can only have one price.");

            if (!Enum.IsDefined(vm.Type))
                ModelState.AddModelError(nameof(vm.Type), "Choose a tour type.");

            if (vm.DestinationId > 0 && !await Db.Destinations.AnyAsync(d => d.Id == vm.DestinationId))
                ModelState.AddModelError(nameof(vm.DestinationId), "This destination no longer exists.");

            if (vm.ActivityTypeId.HasValue && !await Db.ActivityTypes.AnyAsync(a => a.Id == vm.ActivityTypeId.Value))
                ModelState.AddModelError(nameof(vm.ActivityTypeId), "This activity type no longer exists.");

            // images
            AdminImages.ValidateUrl(ModelState, nameof(vm.CoverImageUrl), vm.CoverImageUrl);
            await AdminImages.ValidateFileAsync(ModelState, nameof(vm.CoverImageUrl), vm.CoverFile);

            var newFiles = vm.NewGalleryFiles.Where(f => f != null && f.Length > 0).ToList();
            if (newFiles.Count > MaxGalleryUploads)
                ModelState.AddModelError(nameof(vm.NewGalleryFiles), $"Upload at most {MaxGalleryUploads} images at a time.");
            else
                foreach (var f in newFiles)
                    await AdminImages.ValidateFileAsync(ModelState, nameof(vm.NewGalleryFiles), f);

            // slug
            string slug;
            var typed = vm.Slug?.Trim();
            if (!string.IsNullOrEmpty(typed))
            {
                slug = typed;
                if (await Db.Tours.AnyAsync(t => t.Slug == typed && t.Id != id))
                    ModelState.AddModelError(nameof(vm.Slug), "This slug is already used by another tour.");
            }
            else if (tour != null && !string.IsNullOrEmpty(tour.Slug))
            {
                slug = tour.Slug;
            }
            else
            {
                slug = await Slugger.UniqueAsync(Slugger.Slugify(vm.TitleEn), s => Db.Tours.AnyAsync(t => t.Slug == s));
            }

            if (!ModelState.IsValid)
            {
                await FillLookups(vm);
                return View("Form", vm);
            }

            // ---- from here on we write
            var savedUrls = new List<string>();      // files written during this request (removed again if the DB save fails)
            var removedUrls = new List<string>();    // files to delete after a successful save
            string? oldCover = tour?.CoverImageUrl;

            try
            {
                var cover = await AdminImages.ResolveAsync(vm.CoverImageUrl, vm.CoverFile, _env, "tours");
                if (vm.CoverFile != null && vm.CoverFile.Length > 0 && cover != null) savedUrls.Add(cover);

                tour ??= new Tour { CreatedAt = DateTime.UtcNow };

                tour.TitleEn = vm.TitleEn.Trim();
                tour.TitleFr = vm.TitleFr.Trim();
                tour.Slug = slug;
                tour.SummaryEn = vm.SummaryEn.Trim();
                tour.SummaryFr = vm.SummaryFr.Trim();
                tour.DescriptionEn = vm.DescriptionEn.OrEmpty();
                tour.DescriptionFr = vm.DescriptionFr.OrEmpty();
                tour.QuoteEn = vm.QuoteEn.Clean();
                tour.QuoteFr = vm.QuoteFr.Clean();
                tour.HighlightsEn = vm.HighlightsEn.Clean();
                tour.HighlightsFr = vm.HighlightsFr.Clean();
                tour.IncludedEn = vm.IncludedEn.Clean();
                tour.IncludedFr = vm.IncludedFr.Clean();
                tour.ExcludedEn = vm.ExcludedEn.Clean();
                tour.ExcludedFr = vm.ExcludedFr.Clean();
                tour.ImportantInfoEn = vm.ImportantInfoEn.Clean();
                tour.ImportantInfoFr = vm.ImportantInfoFr.Clean();
                tour.AvailabilityEn = vm.AvailabilityEn.Clean();
                tour.AvailabilityFr = vm.AvailabilityFr.Clean();
                tour.PaymentNoteEn = vm.PaymentNoteEn.Clean();
                tour.PaymentNoteFr = vm.PaymentNoteFr.Clean();
                tour.DurationHours = vm.DurationDays <= 1 ? vm.DurationHours : null;
                tour.Type = vm.Type;
                tour.DurationDays = vm.DurationDays;
                tour.PriceFrom = vm.PriceFrom;
                tour.Currency = vm.Currency;
                tour.CoverImageUrl = cover;
                tour.IsPrivate = vm.IsPrivate;
                tour.IsFeatured = vm.IsFeatured;
                tour.IsActive = vm.IsActive;
                tour.DestinationId = vm.DestinationId;
                tour.ActivityTypeId = vm.ActivityTypeId;

                if (isNew) Db.Tours.Add(tour);
                else tour.UpdatedAt = DateTime.UtcNow;

                // itinerary: replace the whole list
                if (!isNew) Db.TourItineraries.RemoveRange(tour.Itinerary.ToList());
                foreach (var r in vm.Itinerary.Where(r => !r.IsBlank).OrderBy(r => r.DayNumber))
                {
                    tour.Itinerary.Add(new TourItinerary
                    {
                        DayNumber = r.DayNumber,
                        TitleEn = r.TitleEn.OrEmpty(),
                        TitleFr = r.TitleFr.OrEmpty(),
                        DescriptionEn = r.DescriptionEn.OrEmpty(),
                        DescriptionFr = r.DescriptionFr.OrEmpty()
                    });
                }

                // group prices: replace the whole list; "Price from" follows the cheapest tier when tiers are filled in
                if (!isNew) Db.TourPrices.RemoveRange(tour.Prices.ToList());
                var tiers = vm.Prices
                    .Where(p => p.PricePerPerson.HasValue && p.Persons >= 1 && p.Persons <= 50)
                    .GroupBy(p => p.Persons).Select(g => g.First())
                    .OrderBy(p => p.Persons).ToList();
                foreach (var t in tiers)
                    tour.Prices.Add(new TourPrice { Persons = t.Persons, PricePerPerson = t.PricePerPerson!.Value });
                if (tiers.Count > 0) tour.PriceFrom = tiers.Min(t => t.PricePerPerson!.Value);

                // gallery: update alt text / order, delete ticked images, append new uploads
                var order = 0;
                if (!isNew)
                {
                    foreach (var row in vm.Gallery)
                    {
                        var img = tour.Images.FirstOrDefault(x => x.Id == row.Id);   // only images of THIS tour
                        if (img == null) continue;

                        if (row.Remove)
                        {
                            removedUrls.Add(img.ImageUrl);
                            Db.TourImages.Remove(img);
                        }
                        else
                        {
                            img.AltTextEn = row.AltTextEn.Clean();
                            img.AltTextFr = row.AltTextFr.Clean();
                            img.DisplayOrder = order++;
                        }
                    }
                }

                foreach (var f in newFiles)
                {
                    var url = await AdminImages.SaveAsync(f, _env, "tours");
                    savedUrls.Add(url);
                    tour.Images.Add(new TourImage { ImageUrl = url, DisplayOrder = order++ });
                }

                await Db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                foreach (var u in savedUrls) AdminImages.DeleteIfUploaded(u, _env);
                ModelState.AddModelError(string.Empty, "The tour could not be saved. The slug may already exist - try another one.");
                await FillLookups(vm);
                return View("Form", vm);
            }

            if (!isNew && oldCover != tour.CoverImageUrl) AdminImages.DeleteIfUploaded(oldCover, _env);
            foreach (var u in removedUrls) AdminImages.DeleteIfUploaded(u, _env);

            Flash(isNew ? $"Tour “{tour.TitleEn}” created." : $"Tour “{tour.TitleEn}” updated.");
            return RedirectToAction(nameof(Index));
        }

        // --------------------------------------------------------- LOOKUPS
        private async Task<List<SelectListItem>> DestinationItems()
        {
            var rows = await Db.Destinations.AsNoTracking()
                .OrderBy(d => d.DisplayOrder).ThenBy(d => d.NameEn)
                .Select(d => new { d.Id, d.NameEn })
                .ToListAsync();
            return rows.Select(d => new SelectListItem(d.NameEn, d.Id.ToString())).ToList();
        }

        private async Task FillLookups(TourFormVm vm)
        {
            vm.DestinationOptions = await DestinationItems();

            var acts = await Db.ActivityTypes.AsNoTracking().OrderBy(a => a.NameEn)
                .Select(a => new { a.Id, a.NameEn }).ToListAsync();
            vm.ActivityTypeOptions = acts.Select(a => new SelectListItem(a.NameEn, a.Id.ToString())).ToList();

            vm.TypeOptions = Enum.GetValues<TourType>()
                .Select(t => new SelectListItem(AdminFormat.Pretty(t.ToString()), t.ToString()))
                .ToList();
        }
    }
}