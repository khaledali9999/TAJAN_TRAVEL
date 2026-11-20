using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Areas.Admin.Models;
using TAJAN_TRAVEL.Areas.Admin.Services;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Areas.Admin.Controllers
{
    public class FaqsController : AdminBaseController
    {
        public FaqsController(ApplicationDbContext db) : base(db) { }

        public async Task<IActionResult> Index(string? q)
        {
            var query = Db.FAQs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var t = q.Trim();
                query = query.Where(f => f.QuestionEn.Contains(t) || f.QuestionFr.Contains(t) || f.AnswerEn.Contains(t));
            }

            ViewData["Q"] = q;
            return View(await query.OrderBy(f => f.DisplayOrder).ThenBy(f => f.Id).ToListAsync());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id, string field, string? q)
        {
            var f = await Db.FAQs.FindAsync(id);
            if (f == null) return NotFound();

            if (field != "active") return BadRequest();
            f.IsActive = !f.IsActive;

            await Db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { q });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new FaqFormVm
            {
                DisplayOrder = (await Db.FAQs.MaxAsync(f => (int?)f.DisplayOrder) ?? 0) + 1
            };
            await FillLookups(vm);
            return View("Form", vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FaqFormVm vm)
        {
            if (!ModelState.IsValid)
            {
                await FillLookups(vm);
                return View("Form", vm);
            }

            var faq = new FAQ();
            Apply(vm, faq);
            Db.FAQs.Add(faq);
            await Db.SaveChangesAsync();

            Flash("FAQ created.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var f = await Db.FAQs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (f == null) return NotFound();

            var vm = new FaqFormVm
            {
                Id = f.Id,
                QuestionEn = f.QuestionEn,
                QuestionFr = f.QuestionFr,
                AnswerEn = f.AnswerEn,
                AnswerFr = f.AnswerFr,
                Category = f.Category,
                CategoryFr = f.CategoryFr,
                DisplayOrder = f.DisplayOrder,
                IsActive = f.IsActive
            };
            await FillLookups(vm);
            return View("Form", vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FaqFormVm vm)
        {
            var f = await Db.FAQs.FirstOrDefaultAsync(x => x.Id == id);
            if (f == null) return NotFound();

            vm.Id = id;
            if (!ModelState.IsValid)
            {
                await FillLookups(vm);
                return View("Form", vm);
            }

            Apply(vm, f);
            await Db.SaveChangesAsync();

            Flash("FAQ updated.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var f = await Db.FAQs.FindAsync(id);
            if (f == null) return NotFound();

            Db.FAQs.Remove(f);
            await Db.SaveChangesAsync();

            Flash("FAQ deleted.");
            return RedirectToAction(nameof(Index));
        }

        private static void Apply(FaqFormVm vm, FAQ f)
        {
            f.QuestionEn = vm.QuestionEn.Trim();
            f.QuestionFr = vm.QuestionFr.Trim();
            f.AnswerEn = vm.AnswerEn.Trim();
            f.AnswerFr = vm.AnswerFr.Trim();
            f.Category = vm.Category.Clean();
            f.CategoryFr = vm.CategoryFr.Clean();
            f.DisplayOrder = vm.DisplayOrder;
            f.IsActive = vm.IsActive;
        }

        private async Task FillLookups(FaqFormVm vm)
        {
            vm.CategorySuggestions = await Db.FAQs.AsNoTracking()
                .Where(f => f.Category != null && f.Category != "")
                .Select(f => f.Category!).Distinct().OrderBy(c => c)
                .ToListAsync();
        }
    }
}
