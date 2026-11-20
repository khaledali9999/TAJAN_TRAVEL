using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TAJAN_TRAVEL.Models.Entities
{
    public enum TourType
    {
        DayTour,
        FullPackage,
        NileCruise,
        DesertCamping,
        SafariByJeep,
        BeachStay
    }

    public class Tour
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string TitleEn { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string TitleFr { get; set; } = string.Empty;

        [Required, MaxLength(250)]
        public string Slug { get; set; } = string.Empty;

        [Required]
        public string SummaryEn { get; set; } = string.Empty;

        [Required]
        public string SummaryFr { get; set; } = string.Empty;

        public string DescriptionEn { get; set; } = string.Empty;
        public string DescriptionFr { get; set; } = string.Empty;

        // ---- Tour page content (all optional: a section is simply hidden on the site when empty) ----
        // "Lines" fields hold one item per line.
        [MaxLength(600)] public string? QuoteEn { get; set; }
        [MaxLength(600)] public string? QuoteFr { get; set; }

        public string? HighlightsEn { get; set; }      // one highlight per line
        public string? HighlightsFr { get; set; }

        public string? IncludedEn { get; set; }        // one item per line
        public string? IncludedFr { get; set; }
        public string? ExcludedEn { get; set; }
        public string? ExcludedFr { get; set; }

        public string? ImportantInfoEn { get; set; }   // one item per line
        public string? ImportantInfoFr { get; set; }

        [MaxLength(120)] public string? AvailabilityEn { get; set; }   // e.g. "Every Day"
        [MaxLength(120)] public string? AvailabilityFr { get; set; }

        [MaxLength(300)] public string? PaymentNoteEn { get; set; }    // e.g. "Pay 50% online and the rest upon arrival."
        [MaxLength(300)] public string? PaymentNoteFr { get; set; }

        // Length of a day tour in hours (only used when DurationDays = 1)
        [Range(1, 24)]
        public int? DurationHours { get; set; }

        public TourType Type { get; set; }

        // Duration in days (1 = day tour)
        [Range(1, 60)]
        public int DurationDays { get; set; } = 1;

        [Column(TypeName = "decimal(10,2)")]
        public decimal PriceFrom { get; set; }

        [MaxLength(10)]
        public string Currency { get; set; } = "USD";

        [MaxLength(300)]
        public string? CoverImageUrl { get; set; }

        public bool IsPrivate { get; set; } = true;
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; } = true;

        public int DestinationId { get; set; }
        public Destination? Destination { get; set; }

        public int? ActivityTypeId { get; set; }
        public ActivityType? ActivityType { get; set; }

        public ICollection<TourImage> Images { get; set; } = new List<TourImage>();
        public ICollection<TourItinerary> Itinerary { get; set; } = new List<TourItinerary>();
        public ICollection<TourPrice> Prices { get; set; } = new List<TourPrice>();
        public ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // ---- culture aware (English / French) - used by the public views ----
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Title => TAJAN_TRAVEL.Localization.Lang.Pick(TitleEn, TitleFr);

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Summary => TAJAN_TRAVEL.Localization.Lang.Pick(SummaryEn, SummaryFr);

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Description => TAJAN_TRAVEL.Localization.Lang.Pick(DescriptionEn, DescriptionFr);
    }
}