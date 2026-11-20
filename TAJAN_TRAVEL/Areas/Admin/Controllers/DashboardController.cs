using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Areas.Admin.Models;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        public DashboardController(ApplicationDbContext db) : base(db) { }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var prevStart = monthStart.AddMonths(-1);

            // Inquiries are the booking pipeline (inquiry-only model, no payments).
            // Amount is an estimate: tour "price from" x (adults + children).
            var rows = await Db.Inquiries.AsNoTracking()
                .Select(i => new
                {
                    i.Status,
                    i.Type,
                    i.CreatedAt,
                    Amount = i.Tour != null ? i.Tour.PriceFrom * (i.Adults + i.Children) : 0m
                })
                .ToListAsync();

            bool IsBooked(InquiryStatus s) => s == InquiryStatus.Confirmed || s == InquiryStatus.Closed;
            bool IsActive(InquiryStatus s) => s == InquiryStatus.Confirmed || s == InquiryStatus.InProgress;

            var booked = rows.Where(r => IsBooked(r.Status)).ToList();
            var bookedCur = booked.Where(r => r.CreatedAt >= monthStart).ToList();
            var bookedPrev = booked.Where(r => r.CreatedAt >= prevStart && r.CreatedAt < monthStart).ToList();

            var revCur = bookedCur.Sum(r => r.Amount);
            var revPrev = bookedPrev.Sum(r => r.Amount);
            var avgCur = bookedCur.Count == 0 ? 0m : bookedCur.Average(r => r.Amount);
            var avgPrev = bookedPrev.Count == 0 ? 0m : bookedPrev.Average(r => r.Amount);

            var activeNow = rows.Count(r => IsActive(r.Status));
            var activeCur = rows.Count(r => IsActive(r.Status) && r.CreatedAt >= monthStart);
            var activePrev = rows.Count(r => IsActive(r.Status) && r.CreatedAt >= prevStart && r.CreatedAt < monthStart);

            var pendingNow = rows.Count(r => r.Status == InquiryStatus.New);
            var pendingCur = rows.Count(r => r.Status == InquiryStatus.New && r.CreatedAt >= monthStart);
            var pendingPrev = rows.Count(r => r.Status == InquiryStatus.New && r.CreatedAt >= prevStart && r.CreatedAt < monthStart);

            var vm = new DashboardViewModel
            {
                Stats = new List<StatCardVm>
                {
                    new("Total Revenue", AdminFormat.Money(revCur), Pct(revCur, revPrev)),
                    new("Active Bookings", activeNow.ToString(), Pct(activeCur, activePrev)),
                    new("Pending Requests", pendingNow.ToString(), Pct(pendingCur, pendingPrev)),
                    new("Avg. Booking Value", AdminFormat.Money(avgCur), Pct(avgCur, avgPrev))
                },
                PendingNewCount = pendingNow,
                PendingTourCount = rows.Count(r => r.Status == InquiryStatus.New && r.Type == InquiryType.TourInquiry),
                PendingCustomCount = rows.Count(r => r.Status == InquiryStatus.New && r.Type == InquiryType.CustomItinerary)
            };

            // Recent bookings (anything past the "New" stage)
            var recent = await Db.Inquiries.AsNoTracking()
                .Where(i => i.Status != InquiryStatus.New)
                .OrderByDescending(i => i.CreatedAt)
                .Take(5)
                .Select(i => new
                {
                    i.Id,
                    i.FullName,
                    i.Status,
                    TourTitle = i.Tour != null ? i.Tour.TitleEn : null,
                    i.PreferredCategory,
                    Amount = i.Tour != null ? i.Tour.PriceFrom * (i.Adults + i.Children) : 0m
                })
                .ToListAsync();

            vm.RecentBookings = recent.Select(r => new BookingRowVm(
                r.Id,
                AdminFormat.Bk(r.Id),
                r.FullName,
                r.TourTitle ?? r.PreferredCategory ?? "Custom Journey",
                r.Amount > 0 ? AdminFormat.Money(r.Amount) : "—",
                AdminFormat.BookingStatus(r.Status))).ToList();

            // Pending requests (latest 3 new inquiries)
            var pending = await Db.Inquiries.AsNoTracking()
                .Include(i => i.Tour).ThenInclude(t => t!.Destination)
                .Where(i => i.Status == InquiryStatus.New)
                .OrderByDescending(i => i.CreatedAt)
                .Take(3)
                .ToListAsync();

            vm.PendingRequests = pending.Select(i => new RequestRowVm(
                i.Id,
                AdminFormat.Rq(i.Id),
                i.FullName,
                AdminFormat.When(i.CreatedAt),
                i.Tour?.Destination?.NameEn ?? i.Tour?.TitleEn ?? i.PreferredCategory ?? "Custom Journey",
                AdminFormat.TypeLabel(i.Type),
                AdminFormat.ReplyUrl(i))).ToList();

            // Upcoming departures: confirmed files starting in the next 7 days
            var today = DateTime.UtcNow.Date;
            var until = today.AddDays(7);
            var deps = await Db.Inquiries.AsNoTracking()
                .Where(i => i.Status == InquiryStatus.Confirmed
                            && i.PreferredDate != null
                            && i.PreferredDate >= today
                            && i.PreferredDate <= until)
                .OrderBy(i => i.PreferredDate)
                .Take(4)
                .Select(i => new
                {
                    i.Id,
                    i.PreferredDate,
                    i.Adults,
                    i.Children,
                    i.Infants,
                    i.FullName,
                    TourTitle = i.Tour != null ? i.Tour.TitleEn : null,
                    Destination = i.Tour != null && i.Tour.Destination != null ? i.Tour.Destination.NameEn : null
                })
                .ToListAsync();

            vm.Departures = deps.Select(d =>
            {
                var guests = d.Adults + d.Children + d.Infants;
                return new DepartureRowVm(
                    d.Id,
                    d.TourTitle ?? "Custom Journey",
                    d.Destination ?? d.FullName,
                    AdminFormat.Day(d.PreferredDate!.Value),
                    guests + (guests == 1 ? " Guest" : " Guests"));
            }).ToList();

            vm.ProposalsPending = await Db.Inquiries.CountAsync(i =>
                i.Status == InquiryStatus.New && i.Type == InquiryType.CustomItinerary);

            // ---- content overview: live counts from the database --------------------------------
            var toursTotal = await Db.Tours.CountAsync();
            var toursActive = await Db.Tours.CountAsync(t => t.IsActive);
            var destTotal = await Db.Destinations.CountAsync();
            var destActive = await Db.Destinations.CountAsync(d => d.IsActive);
            var storiesPub = await Db.BlogPosts.CountAsync(b => b.IsPublished);
            var storiesDraft = await Db.BlogPosts.CountAsync(b => !b.IsPublished);
            var eventsTotal = await Db.TravelEvents.CountAsync();
            var eventsActive = await Db.TravelEvents.CountAsync(e => e.IsActive);
            var reviewsTotal = await Db.Testimonials.CountAsync();
            var reviewsPending = await Db.Testimonials.CountAsync(t => !t.IsApproved);
            var faqsTotal = await Db.FAQs.CountAsync();
            var faqsActive = await Db.FAQs.CountAsync(f => f.IsActive);

            vm.Content = new List<ContentStatVm>
            {
                new("Tours", toursTotal.ToString(), toursActive + " live on site", "map", "Tours"),
                new("Destinations", destTotal.ToString(), destActive + " live on site", "map-pin", "Destinations"),
                new("Stories", (storiesPub + storiesDraft).ToString(), storiesPub + " published · " + storiesDraft + " drafts", "book-open", "Stories"),
                new("Events", eventsTotal.ToString(), eventsActive + " live on site", "calendar", "Events"),
                new("Reviews", reviewsTotal.ToString(), reviewsPending + " awaiting approval", "star", "Reviews"),
                new("FAQs", faqsTotal.ToString(), faqsActive + " live on site", "help-circle", "Faqs")
            };

            // ---- last 6 months: requests received + estimated revenue of booked files --------------
            for (var back = 5; back >= 0; back--)
            {
                var from = monthStart.AddMonths(-back);
                var to = from.AddMonths(1);
                var inMonth = rows.Where(r => r.CreatedAt >= from && r.CreatedAt < to).ToList();
                vm.Trend.Add(new TrendPointVm(
                    from.ToString("MMM", CultureInfo.InvariantCulture),
                    inMonth.Count,
                    inMonth.Where(r => IsBooked(r.Status)).Sum(r => r.Amount)));
            }

            // ---- pipeline by status ------------------------------------------------------------------
            foreach (var st in Enum.GetValues<InquiryStatus>())
            {
                vm.StatusBreakdown.Add(new StatusSliceVm(
                    AdminFormat.Pretty(st.ToString()),
                    rows.Count(r => r.Status == st),
                    st switch
                    {
                        InquiryStatus.New => "bg-amber-400",
                        InquiryStatus.Contacted => "bg-sky-400",
                        InquiryStatus.InProgress => "bg-yellow-500",
                        InquiryStatus.Confirmed => "bg-emerald-500",
                        _ => "bg-slate-400"
                    }));
            }

            // ---- most requested tours ----------------------------------------------------------------
            vm.TopTours = (await Db.Tours.AsNoTracking()
                    .Select(t => new { t.Id, t.TitleEn, Count = t.Inquiries.Count })
                    .Where(x => x.Count > 0)
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToListAsync())
                .Select(x => new TopTourVm(x.Id, x.TitleEn, x.Count))
                .ToList();

            return View(vm);
        }

        private static decimal Pct(decimal cur, decimal prev) =>
            prev == 0 ? (cur == 0 ? 0 : 100) : Math.Round((cur - prev) / prev * 100m, 0);
    }
}
