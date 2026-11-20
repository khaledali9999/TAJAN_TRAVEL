using System.ComponentModel.DataAnnotations;

namespace TAJAN_TRAVEL.Models.Entities
{
    public class Destination
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string NameEn { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string NameFr { get; set; } = string.Empty;

        // Short tagline shown on cards
        [Required]
        public string DescriptionEn { get; set; } = string.Empty;

        [Required]
        public string DescriptionFr { get; set; } = string.Empty;

        // Long paragraph shown on the Details page
        public string LongDescriptionEn { get; set; } = string.Empty;
        public string LongDescriptionFr { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? CoverImageUrl { get; set; }

        // Large wide image for the destination page hero (falls back to CoverImageUrl when empty)
        [MaxLength(300)]
        public string? HeroImageUrl { get; set; }

        // e.g. "Oct � April"
        [MaxLength(100)]
        public string? BestTime { get; set; }

        [MaxLength(100)]
        public string? BestTimeFr { get; set; }

        // e.g. "Ancient Wonders"
        [MaxLength(150)]
        public string? KnownFor { get; set; }

        [MaxLength(150)]
        public string? KnownForFr { get; set; }

        // e.g. Nile, Desert, Coastal, City & Culture
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? CategoryFr { get; set; }

        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Tour> Tours { get; set; } = new List<Tour>();

        // ---- culture aware (English / French) - used by the public views ----
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Name => TAJAN_TRAVEL.Localization.Lang.Pick(NameEn, NameFr);

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Description => TAJAN_TRAVEL.Localization.Lang.Pick(DescriptionEn, DescriptionFr);

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string LongDescription => TAJAN_TRAVEL.Localization.Lang.Pick(LongDescriptionEn, LongDescriptionFr);
    }
}