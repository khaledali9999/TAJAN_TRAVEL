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
    public class ReviewsController : AdminBaseController
    {
        private readonly IWebHostEnvironment _env;

        public ReviewsController(ApplicationDbContext db, IWebHostEnvironment env) : base(db)
        {
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var list = await Db.Testimonials.AsNoTracking()
                .Include(t => t.Tour)
                .OrderBy(t => t.IsApproved)               // pending first
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(list);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id, string field)
        {
            var t = await Db.Testimonials.FindAsync(id);
            if (t == null) return NotFound();

            switch (field)
            {
                case "approved": t.IsApproved = !t.IsApproved; break;
                case "featured": t.IsFeatured = !t.IsFeatured; break;
                default: return BadRequest();
            }

            await Db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new ReviewFormVm
            {
                DisplayOrder = (await Db.Testimonials.MaxAsync(t => (int?)t.DisplayOrder) ?? 0) + 1
            };
            await FillLookups(vm);
            return View("Form", vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public Task<IActionResult> Create(ReviewFormVm vm) => SaveAsync(vm, null);

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var t = await Db.Testimonials.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound();

            var vm = new ReviewFormVm
            {
                Id = t.Id,
                ClientName = t.ClientName,
                ClientCountry = t.ClientCountry,
                ClientCountryFr = t.ClientCountryFr,
                ClientPhotoUrl = t.ClientPhotoUrl,
                QuoteEn = t.QuoteEn,
                QuoteFr = t.QuoteFr,
                Rating = t.Rating,
                TourId = t.TourId,
                DisplayOrder = t.DisplayOrder,
                IsFeatured = t.IsFeatured,
                IsApproved = t.IsApproved
            };
            await FillLookups(vm);
            return View("Form", vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReviewFormVm vm)
        {
            var t = await Db.Testimonials.FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound();

            vm.Id = id;
            return await SaveAsync(vm, t);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var t = await Db.Testimonials.FindAsync(id);
            if (t == null) return NotFound();

            var photo = t.ClientPhotoUrl;
            Db.Testimonials.Remove(t);
            await Db.SaveChangesAsync();
            AdminImages.DeleteIfUploaded(photo, _env);

            Flash("Review deleted.");
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> SaveAsync(ReviewFormVm vm, Testimonial? review)
        {
            var isNew = review == null;

            if (vm.TourId.HasValue && !await Db.Tours.AnyAsync(t => t.Id == vm.TourId.Value))
                ModelState.AddModelError(nameof(vm.TourId), "This tour no longer exists.");

            AdminImages.ValidateUrl(ModelState, nameof(vm.ClientPhotoUrl), vm.ClientPhotoUrl);
            await AdminImages.ValidateFileAsync(ModelState, nameof(vm.ClientPhotoUrl), vm.PhotoFile);

            if (!ModelState.IsValid)
            {
                await FillLookups(vm);
                return View("Form", vm);
            }

            var oldPhoto = review?.ClientPhotoUrl;
            var photo = await AdminImages.ResolveAsync(vm.ClientPhotoUrl, vm.PhotoFile, _env, "reviews");

            review ??= new Testimonial { CreatedAt = DateTime.UtcNow };

            review.ClientName = vm.ClientName.Trim();
            review.ClientCountry = vm.ClientCountry.Clean();
            review.ClientCountryFr = vm.ClientCountryFr.Clean();
            review.ClientPhotoUrl = photo;
            review.QuoteEn = vm.QuoteEn.Trim();
            review.QuoteFr = vm.QuoteFr.Trim();
            review.Rating = vm.Rating;
            review.TourId = vm.TourId;
            review.DisplayOrder = vm.DisplayOrder;
            review.IsFeatured = vm.IsFeatured;
            review.IsApproved = vm.IsApproved;

            if (isNew) Db.Testimonials.Add(review);
            await Db.SaveChangesAsync();

            if (!isNew && oldPhoto != review.ClientPhotoUrl) AdminImages.DeleteIfUploaded(oldPhoto, _env);

            Flash(isNew ? "Review added." : "Review updated.");
            return RedirectToAction(nameof(Index));
        }

        private async Task FillLookups(ReviewFormVm vm)
        {
            var tours = await Db.Tours.AsNoTracking().OrderBy(t => t.TitleEn)
                .Select(t => new { t.Id, t.TitleEn }).ToListAsync();
            vm.TourOptions = tours.Select(t => new SelectListItem(t.TitleEn, t.Id.ToString())).ToList();
        }
    }
}
