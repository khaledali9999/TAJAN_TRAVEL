using Microsoft.AspNetCore.Mvc;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.Entities;
using TAJAN_TRAVEL.Models.ViewModels;

namespace TAJAN_TRAVEL.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ContactViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var message = new ContactMessage
            {
                FullName = $"{model.FirstName} {model.LastName}".Trim(),
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Subject = "Website inquiry",
                Message = model.Message
            };

            _context.ContactMessages.Add(message);
            await _context.SaveChangesAsync();

            // TODO (Integrations phase): trigger EmailService / WhatsAppService notification here.

            TempData["ContactSuccess"] = true;
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// "Let's Work Together" popup (AJAX). Stored as a ContactMessage with a "Partnership request" subject,
        /// so it shows up in the admin Notifications list without needing a new table/migration.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Partner(PartnerRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kv => kv.Value?.Errors.Count > 0)
                    .ToDictionary(kv => kv.Key, kv => kv.Value!.Errors[0].ErrorMessage);
                return BadRequest(new { success = false, errors });
            }

            var company = string.IsNullOrWhiteSpace(model.Company) ? null : model.Company.Trim();
            var subject = company != null ? $"Partnership request – {company}" : "Partnership request";
            if (subject.Length > 150) subject = subject[..150];

            var body =
                $"Company: {company ?? "-"}\n" +
                $"Country: {model.Country}\n" +
                $"Phone / WhatsApp: {model.Phone}\n\n" +
                model.Message;

            _context.ContactMessages.Add(new ContactMessage
            {
                FullName = model.FullName.Trim(),
                Email = model.BusinessEmail.Trim(),
                PhoneNumber = model.Phone.Trim(),
                Subject = subject,
                Message = body
            });
            await _context.SaveChangesAsync();

            // TODO (Integrations phase): trigger EmailService / WhatsAppService notification here.

            return Ok(new { success = true });
        }
    }
}