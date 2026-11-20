using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace TAJAN_TRAVEL.Models.Entities
{
    public class TravelEvent
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string TitleEn { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string TitleFr { get; set; } = string.Empty;

        [Required, MaxLength(250)]
        public string Slug { get; set; } = string.Empty;

        // Short text shown on the card
        [Required]
        public string SummaryEn { get; set; } = string.Empty;

        [Required]
        public string SummaryFr { get; set; } = string.Empty;

        // Longer text shown in the details modal
        [Required]
        public string DescriptionEn { get; set; } = string.Empty;

        [Required]
        public string DescriptionFr { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? CoverImageUrl { get; set; }

        public int? EventCategoryId { get; set; }
        public EventCategory? Category { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        [MaxLength(200)]
        public string? LocationFr { get; set; }

        // Real date (used for "This Month" filter + sorting)
        public DateTime? EventDate { get; set; }

        // Used instead of a date when it is not fixed, e.g. "On request" / "Dates to be announced"
        [MaxLength(100)]
        public string? DateLabel { get; set; }

        public int ReadTimeMinutes { get; set; } = 5;

        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // "12 OCT 2026" / "12 OCT. 2026"
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string ShortDate =>
            EventDate.HasValue ? TAJAN_TRAVEL.Localization.Lang.DayMonthShort(EventDate.Value)
            : (DateLabel != null ? TAJAN_TRAVEL.Localization.Lang.T(DateLabel).ToUpperInvariant() : TAJAN_TRAVEL.Localization.Lang.T("TBA"));

        // "August 2, 2027" / "2 ao�t 2027"
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string LongDate =>
            EventDate.HasValue ? TAJAN_TRAVEL.Localization.Lang.LongDate(EventDate.Value)
            : (DateLabel != null ? TAJAN_TRAVEL.Localization.Lang.T(DateLabel) : TAJAN_TRAVEL.Localization.Lang.T("Dates to be announced"));


        // ---- culture aware (English / French) - used by the public views ----
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Title => TAJAN_TRAVEL.Localization.Lang.Pick(TitleEn, TitleFr);

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Summary => TAJAN_TRAVEL.Localization.Lang.Pick(SummaryEn, SummaryFr);

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Description => TAJAN_TRAVEL.Localization.Lang.Pick(DescriptionEn, DescriptionFr);
    }
}
