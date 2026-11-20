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
    // "Stories & Journal" = BlogPost
    public class StoriesController : AdminBaseController
    {
        private const int PageSize = 15;
        private readonly IWebHostEnvironment _env;

        public StoriesController(ApplicationDbContext db, IWebHostEnvironment env) : base(db)
        {
            _env = env;
        }

        public async Task<IActionResult> Index(string? q, string? status, int page = 1)
        {
            var query = Db.BlogPosts.AsNoTracking().Include(b => b.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var t = q.Trim();
                query = query.Where(b => b.TitleEn.Contains(t) || b.TitleFr.Contains(t) || (b.Author != null && b.Author.Contains(t)));
            }

            switch (status)
            {
                case "published": query = query.Where(b => b.IsPublished); break;
                case "draft": query = query.Where(b => !b.IsPublished); break;
                default: status = null; break;
            }

            var total = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
            page = Math.Clamp(page, 1, totalPages);

            var items = await query
                .OrderByDescending(b => b.PublishedAt ?? b.CreatedAt)
                .Skip((page - 1) * PageSize).Take(PageSize)
                .ToListAsync();

            return View(new StoriesIndexVm
            {
                Items = items,
                Q = q,
                Status = status,
                Page = page,
                TotalPages = totalPages,
                Total = total,
                PublishedCount = await Db.BlogPosts.CountAsync(b => b.IsPublished),
                DraftCount = await Db.BlogPosts.CountAsync(b => !b.IsPublished)
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id, string field, string? q, string? status, int page = 1)
        {
            var b = await Db.BlogPosts.FindAsync(id);
            if (b == null) return NotFound();

            switch (field)
            {
                case "published":
                    b.IsPublished = !b.IsPublished;
                    if (b.IsPublished && b.PublishedAt == null) b.PublishedAt = DateTime.UtcNow;
                    break;
                case "featured": b.IsFeatured = !b.IsFeatured; break;
                default: return BadRequest();
            }

            b.UpdatedAt = DateTime.UtcNow;
            await Db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { q, status, page });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new StoryFormVm { Author = "TAJAN TRAVEL" };
            await FillLookups(vm);
            return View("Form", vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public Task<IActionResult> Create(StoryFormVm vm) => SaveAsync(vm, null);

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var b = await Db.BlogPosts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (b == null) return NotFound();

            var vm = new StoryFormVm
            {
                Id = b.Id,
                TitleEn = b.TitleEn,
                TitleFr = b.TitleFr,
                Slug = b.Slug,
                SummaryEn = b.SummaryEn,
                SummaryFr = b.SummaryFr,
                ContentEn = b.ContentEn,
                ContentFr = b.ContentFr,
                CoverImageUrl = b.CoverImageUrl,
                CoverImageCaption = b.CoverImageCaption,
                CoverImageCaptionFr = b.CoverImageCaptionFr,
                ReadTimeMinutes = b.ReadTimeMinutes,
                BlogCategoryId = b.BlogCategoryId,
                Author = b.Author,
                IsFeatured = b.IsFeatured,
                IsPublished = b.IsPublished,
                PublishedAt = b.PublishedAt
            };
            await FillLookups(vm);
            return View("Form", vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StoryFormVm vm)
        {
            var b = await Db.BlogPosts.FirstOrDefaultAsync(x => x.Id == id);
            if (b == null) return NotFound();

            vm.Id = id;
            return await SaveAsync(vm, b);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var b = await Db.BlogPosts.FindAsync(id);
            if (b == null) return NotFound();

            var cover = b.CoverImageUrl;
            var title = b.TitleEn;

            Db.BlogPosts.Remove(b);
            await Db.SaveChangesAsync();
            AdminImages.DeleteIfUploaded(cover, _env);

            Flash($"Story “{title}” deleted.");
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> SaveAsync(StoryFormVm vm, BlogPost? post)
        {
            var id = post?.Id ?? 0;
            var isNew = post == null;

            if (vm.BlogCategoryId.HasValue && !await Db.BlogCategories.AnyAsync(c => c.Id == vm.BlogCategoryId.Value))
                ModelState.AddModelError(nameof(vm.BlogCategoryId), "This category no longer exists.");

            AdminImages.ValidateUrl(ModelState, nameof(vm.CoverImageUrl), vm.CoverImageUrl);
            await AdminImages.ValidateFileAsync(ModelState, nameof(vm.CoverImageUrl), vm.CoverFile);

            string slug;
            var typed = vm.Slug?.Trim();
            if (!string.IsNullOrEmpty(typed))
            {
                slug = typed;
                if (await Db.BlogPosts.AnyAsync(x => x.Slug == typed && x.Id != id))
                    ModelState.AddModelError(nameof(vm.Slug), "This slug is already used by another story.");
            }
            else if (post != null && !string.IsNullOrEmpty(post.Slug))
            {
                slug = post.Slug;
            }
            else
            {
                slug = await Slugger.UniqueAsync(Slugger.Slugify(vm.TitleEn), s => Db.BlogPosts.AnyAsync(x => x.Slug == s));
            }

            if (!ModelState.IsValid)
            {
                await FillLookups(vm);
                return View("Form", vm);
            }

            string? savedUrl = null;
            var oldCover = post?.CoverImageUrl;

            try
            {
                var cover = await AdminImages.ResolveAsync(vm.CoverImageUrl, vm.CoverFile, _env, "stories");
                if (vm.CoverFile != null && vm.CoverFile.Length > 0) savedUrl = cover;

                post ??= new BlogPost { CreatedAt = DateTime.UtcNow };

                post.TitleEn = vm.TitleEn.Trim();
                post.TitleFr = vm.TitleFr.Trim();
                post.Slug = slug;
                post.SummaryEn = vm.SummaryEn.Trim();
                post.SummaryFr = vm.SummaryFr.Trim();
                post.ContentEn = vm.ContentEn.Trim();
                post.ContentFr = vm.ContentFr.Trim();
                post.CoverImageUrl = cover;
                post.CoverImageCaption = vm.CoverImageCaption.Clean();
                post.CoverImageCaptionFr = vm.CoverImageCaptionFr.Clean();
                post.ReadTimeMinutes = vm.ReadTimeMinutes;
                post.BlogCategoryId = vm.BlogCategoryId;
                post.Author = vm.Author.Clean();
                post.IsFeatured = vm.IsFeatured;
                post.IsPublished = vm.IsPublished;

                // the public list is sorted by PublishedAt, so a published story always needs one
                post.PublishedAt = vm.IsPublished
                    ? (vm.PublishedAt ?? post.PublishedAt ?? DateTime.UtcNow)
                    : vm.PublishedAt;

                if (isNew) Db.BlogPosts.Add(post);
                else post.UpdatedAt = DateTime.UtcNow;

                await Db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                AdminImages.DeleteIfUploaded(savedUrl, _env);
                ModelState.AddModelError(string.Empty, "The story could not be saved. The slug may already exist - try another one.");
                await FillLookups(vm);
                return View("Form", vm);
            }

            if (!isNew && oldCover != post.CoverImageUrl) AdminImages.DeleteIfUploaded(oldCover, _env);

            Flash(isNew ? $"Story “{post.TitleEn}” created." : $"Story “{post.TitleEn}” updated.");
            return RedirectToAction(nameof(Index));
        }

        private async Task FillLookups(StoryFormVm vm)
        {
            var cats = await Db.BlogCategories.AsNoTracking()
                .OrderBy(c => c.DisplayOrder).ThenBy(c => c.NameEn)
                .Select(c => new { c.Id, c.NameEn }).ToListAsync();
            vm.CategoryOptions = cats.Select(c => new SelectListItem(c.NameEn, c.Id.ToString())).ToList();
        }
    }
}
