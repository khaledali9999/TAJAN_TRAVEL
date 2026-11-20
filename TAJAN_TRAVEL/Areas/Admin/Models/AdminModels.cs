using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Areas.Admin.Models
{
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }

    public class NavItem
    {
        public string Icon { get; set; } = "circle";
        public string Label { get; set; } = string.Empty;
        public string Controller { get; set; } = "Dashboard";
        public string Action { get; set; } = "Index";
        public string? Status { get; set; }
        public string? Type { get; set; }    // TourRequests only: TourInquiry | CustomItinerary
        public string? Title { get; set; }   // used by ComingSoon pages
        public int Badge { get; set; }
    }

    public record ToggleVm(string Controller, int Id, string Field, bool On, string? Q = null);

    public record StatCardVm(string Title, string Value, decimal Change);
    public record BookingRowVm(int InquiryId, string Code, string Customer, string Tour, string Amount, string Status);
    public record RequestRowVm(int InquiryId, string Code, string Customer, string When, string Place, string Tag, string ReplyUrl);
    public record DepartureRowVm(int InquiryId, string Title, string Sub, string Date, string Guests);

    public class DashboardViewModel
    {
        public List<StatCardVm> Stats { get; set; } = new();
        public List<BookingRowVm> RecentBookings { get; set; } = new();
        public List<RequestRowVm> PendingRequests { get; set; } = new();
        public int PendingNewCount { get; set; }
        public int PendingTourCount { get; set; }      // new requests sent from a tour details page
        public int PendingCustomCount { get; set; }    // new requests sent from "Design your Egypt tour"
        public List<DepartureRowVm> Departures { get; set; } = new();
        public int ProposalsPending { get; set; }

        // live overview blocks (all read from the database)
        public List<ContentStatVm> Content { get; set; } = new();
        public List<TrendPointVm> Trend { get; set; } = new();
        public List<StatusSliceVm> StatusBreakdown { get; set; } = new();
        public List<TopTourVm> TopTours { get; set; } = new();
    }

    public class TourRequestsIndexViewModel
    {
        public List<Inquiry> Items { get; set; } = new();
        public string? Status { get; set; }
        public string? Type { get; set; }
        public string? Q { get; set; }
        public int Page { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int Total { get; set; }
        public Dictionary<InquiryStatus, int> Counts { get; set; } = new();       // per status (within the selected type)
        public Dictionary<InquiryType, int> TypeCounts { get; set; } = new();     // per request source
    }

    public class DestinationRowVm
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameFr { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public int ToursCount { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
    }

    public static class AdminFormat
    {
        static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
        static readonly TimeZoneInfo Tz = ResolveTz();

        static TimeZoneInfo ResolveTz()
        {
            foreach (var id in new[] { "Africa/Cairo", "Egypt Standard Time" })
            {
                try { return TimeZoneInfo.FindSystemTimeZoneById(id); } catch { }
            }
            return TimeZoneInfo.Utc;
        }

        public static DateTime ToLocal(DateTime utc) =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Tz);

        public static string Money(decimal v) => "$" + v.ToString("N0", Inv);
        public static string Num(decimal v) => v.ToString("N0", Inv);
        public static string Bk(int id) => "BK-" + id.ToString("0000");
        public static string Rq(int id) => "RQ-" + id.ToString("0000");
        public static string Day(DateTime d) => d.ToString("MMM d", Inv);
        public static string FullDay(DateTime d) => d.ToString("MMM d, yyyy", Inv);

        public static string When(DateTime utc)
        {
            var d = ToLocal(utc);
            var today = ToLocal(DateTime.UtcNow).Date;
            if (d.Date == today) return "Today, " + d.ToString("HH:mm", Inv);
            if (d.Date == today.AddDays(-1)) return "Yesterday, " + d.ToString("HH:mm", Inv);
            return d.ToString("MMM d, HH:mm", Inv);
        }

        public static string Pretty(string s) => Regex.Replace(s, "(?<=[a-z])(?=[A-Z])", " ");

        public static string BookingStatus(InquiryStatus s) => s switch
        {
            InquiryStatus.Confirmed => "Confirmed",
            InquiryStatus.Closed => "Completed",
            _ => "Pending"
        };

        public static string BookingPill(string status) => status switch
        {
            "Confirmed" => "bg-emerald-100 text-emerald-700",
            "Completed" => "bg-slate-100 text-slate-600",
            _ => "bg-yellow-100 text-yellow-800"
        };

        public static string StatusPill(InquiryStatus s) => s switch
        {
            InquiryStatus.New => "bg-amber-100 text-amber-800",
            InquiryStatus.Contacted => "bg-sky-100 text-sky-800",
            InquiryStatus.InProgress => "bg-yellow-100 text-yellow-800",
            InquiryStatus.Confirmed => "bg-emerald-100 text-emerald-700",
            _ => "bg-slate-100 text-slate-600"
        };

        public static string TypeLabel(InquiryType t) => t switch
        {
            InquiryType.CustomItinerary => "Design Your Tour",
            InquiryType.TourInquiry => "Tour Details",
            _ => "General"
        };

        public static string Digits(string? phone) =>
            new string((phone ?? string.Empty).Where(char.IsDigit).ToArray());

        public static string WhatsAppUrl(string? phone) => "https://wa.me/" + Digits(phone);

        public static string MailUrl(string email, string code) =>
            "mailto:" + email + "?subject=" + Uri.EscapeDataString("Your TAJAN TRAVEL inquiry " + code);

        public static string ReplyUrl(Inquiry i) =>
            i.PreferredContact == InquiryContactMethod.WhatsApp
                ? WhatsAppUrl(i.PhoneNumber)
                : MailUrl(i.Email, Rq(i.Id));
    }
}
