using System.ComponentModel.DataAnnotations;

namespace TAJAN_TRAVEL.Models.Entities
{
    public class TourItinerary
    {
        public int Id { get; set; }

        public int TourId { get; set; }
        public Tour? Tour { get; set; }

        [Range(1, 60)]
        public int DayNumber { get; set; }

        [Required, MaxLength(150)]
        public string TitleEn { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string TitleFr { get; set; } = string.Empty;

        public string DescriptionEn { get; set; } = string.Empty;
        public string DescriptionFr { get; set; } = string.Empty;

        // ---- culture aware (English / French) - used by the public views ----
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Title => TAJAN_TRAVEL.Localization.Lang.Pick(TitleEn, TitleFr);

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Description => TAJAN_TRAVEL.Localization.Lang.Pick(DescriptionEn, DescriptionFr);
    }
}
