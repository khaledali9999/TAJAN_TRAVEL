using System.ComponentModel.DataAnnotations;

namespace TAJAN_TRAVEL.Models.Entities
{
    public class BlogPost
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

        [Required]
        public string ContentEn { get; set; } = string.Empty;

        [Required]
        public string ContentFr { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? CoverImageUrl { get; set; }

        // Small caption shown under the image in the story popup, e.g. "Morning light along Al-Muizz Street, Cairo."
        [MaxLength(200)]
        public string? CoverImageCaption { get; set; }

        [MaxLength(200)]
        public string? CoverImageCaptionFr { get; set; }

        public int ReadTimeMinutes { get; set; } = 5;

        // Shown as the big "Featured Story" on the Stories page
        public bool IsFeatured { get; set; }

        public int? BlogCategoryId { get; set; }
        public BlogCategory? Category { get; set; }

        [MaxLength(100)]
        public string? Author { get; set; }

        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // ---- culture aware (English / French) - used by the public views ----
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Title => TAJAN_TRAVEL.Localization.Lang.Pick(TitleEn, TitleFr);

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Summary => TAJAN_TRAVEL.Localization.Lang.Pick(SummaryEn, SummaryFr);

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Content => TAJAN_TRAVEL.Localization.Lang.Pick(ContentEn, ContentFr);
    }
}