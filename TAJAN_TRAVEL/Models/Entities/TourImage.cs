using System.ComponentModel.DataAnnotations;

namespace TAJAN_TRAVEL.Models.Entities
{
    public class TourImage
    {
        public int Id { get; set; }

        public int TourId { get; set; }
        public Tour? Tour { get; set; }

        [Required, MaxLength(300)]
        public string ImageUrl { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? AltTextEn { get; set; }

        [MaxLength(200)]
        public string? AltTextFr { get; set; }

        public int DisplayOrder { get; set; }

        // ---- culture aware (English / French) - used by the public views ----
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string AltText => TAJAN_TRAVEL.Localization.Lang.Pick(AltTextEn, AltTextFr);
    }
}
