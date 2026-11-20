using TAJAN_TRAVEL.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.Entities;
using TAJAN_TRAVEL.Models.ViewModels;

namespace TAJAN_TRAVEL.Controllers
{
    public class InquiryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InquiryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /Inquiry/Create
        // GET /Inquiry/Create?tourId=13
        [HttpGet]
        public async Task<IActionResult> Create(int? tourId, string? eventTitle)
        {
            var vm = new InquiryViewModel();

            if (tourId.HasValue)
            {
                var tour = await _context.Tours
                    .FirstOrDefaultAsync(t => t.Id == tourId.Value && t.IsActive);

                if (tour != null)
                {
                    vm.TourId = tour.Id;
                    vm.TourTitle = tour.Title;
                }
            }

            // Coming from the Events page ("Contact Us For More Info")
            if (!tourId.HasValue && !string.IsNullOrWhiteSpace(eventTitle))
            {
                var title = eventTitle.Trim();
                if (title.Length > 200) title = title[..200];
                vm.TourTitle = title;
                vm.AdditionalNotes = $"I'd like more information about: {title}";
            }

            return View(vm);
        }

        // POST /Inquiry/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InquiryViewModel model)
        {
            if (model.FromDate.HasValue && model.ToDate.HasValue && model.ToDate < model.FromDate)
            {
                ModelState.AddModelError(nameof(model.ToDate), Lang.T("End date must be on or after the start date."));
            }

            // country / language must be one of the offered options
            if (!string.IsNullOrWhiteSpace(model.Country) &&
                !InquiryOptions.Countries.Contains(model.Country, StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.Country), Lang.T("Please select your country."));
            }
            if (!string.IsNullOrWhiteSpace(model.Language) &&
                !InquiryOptions.Languages.Contains(model.Language, StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.Language), Lang.T("Please select a language."));
            }

            // children: one age (2-12) per child (no limit on how many children)
            if (model.ChildrenAges.Any(a => a < 2 || a > 12))
            {
                ModelState.AddModelError(nameof(model.ChildrenAges), Lang.T("Please enter an age between 2 and 12."));
            }
            else if (model.ChildrenAges.Count != model.Children)
            {
                ModelState.AddModelError(nameof(model.ChildrenAges), Lang.T("Please enter the age of each child."));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var contact = model.PreferredContacts
                .Select(c => Enum.TryParse<InquiryContactMethod>(c, true, out var m) ? m : 0)
                .Aggregate((InquiryContactMethod)0, (a, b) => a | b)
                & (InquiryContactMethod.Email | InquiryContactMethod.WhatsApp);
            if (contact == 0) contact = InquiryContactMethod.Email;

            var inquiry = new Inquiry
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = BuildPhoneNumber(model.CountryCode, model.PhoneNumber),
                PreferredDate = model.FromDate,
                PreferredEndDate = model.ToDate,
                DateFlexibility = Enum.Parse<TripDateFlexibility>(model.Flexibility.ToString()),
                Adults = model.Adults,
                Children = model.Children,
                Infants = model.Infants,
                ChildrenAges = model.Children > 0 ? string.Join(", ", model.ChildrenAges) : null,
                Country = model.Country?.Trim(),
                Language = model.Language?.Trim(),
                GroupSize = Math.Max(1, model.Adults + model.Children),
                PreferredCategory = model.PreferredCategories.Count > 0
                    ? string.Join(", ", model.PreferredCategories
                        .Where(c => new InquiryViewModel().Categories.Contains(c))
                        .Distinct())
                    : null,
                PreferredContact = contact,
                AdditionalNotes = model.AdditionalNotes?.Trim(),
                Type = model.TourId.HasValue ? InquiryType.TourInquiry : InquiryType.CustomItinerary,
                TourId = model.TourId,
                Status = InquiryStatus.New,
                CreatedAt = DateTime.UtcNow
            };

            _context.Inquiries.Add(inquiry);
            await _context.SaveChangesAsync();

            // TODO (Integrations phase): trigger EmailService / WhatsAppService notification here.

            TempData["InquirySuccess"] = true;
            return RedirectToAction(nameof(Success));
        }

        // Country code + number, e.g. "+20 1012345678".
        // - the local leading zero is dropped (01012345678 -> 1012345678)
        // - a number already typed in international format (+33... / 0033...) is kept as is,
        //   so forms without a country-code field (e.g. the tour page) don't get a wrong prefix.
        private static string BuildPhoneNumber(string? countryCode, string? rawPhone)
        {
            var phone = (rawPhone ?? string.Empty).Trim();

            if (phone.StartsWith("+"))
                return phone;

            if (phone.StartsWith("00"))
                return "+" + phone[2..].TrimStart();

            var code = (countryCode ?? string.Empty).Trim();
            return $"{code} {phone.TrimStart('0')}".Trim();
        }

        // GET /Inquiry/Success
        [HttpGet]
        public IActionResult Success()
        {
            if (TempData["InquirySuccess"] == null)
            {
                return RedirectToAction(nameof(Create));
            }

            return View();
        }
    }
}