using System.ComponentModel.DataAnnotations;

namespace TAJAN_TRAVEL.Models.Entities
{
    public class EventCategory
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string NameEn { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string NameFr { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Slug { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public ICollection<TravelEvent> Events { get; set; } = new List<TravelEvent>();

        // ---- culture aware (English / French) - used by the public views ----
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Name => TAJAN_TRAVEL.Localization.Lang.Pick(NameEn, NameFr);
    }
}
