using System.ComponentModel.DataAnnotations;

namespace TAJAN_TRAVEL.Models.Entities
{
    public class Testimonial
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string ClientName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ClientCountry { get; set; }

        [MaxLength(100)]
        public string? ClientCountryFr { get; set; }

        [MaxLength(300)]
        public string? ClientPhotoUrl { get; set; }

        [Required]
        public string QuoteEn { get; set; } = string.Empty;

        [Required]
        public string QuoteFr { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Rating { get; set; } = 5;

        public int? TourId { get; set; }
        public Tour? Tour { get; set; }

        public bool IsFeatured { get; set; }
        public bool IsApproved { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ---- culture aware (English / French) - used by the public views ----
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Quote => TAJAN_TRAVEL.Localization.Lang.Pick(QuoteEn, QuoteFr);
    }
}
