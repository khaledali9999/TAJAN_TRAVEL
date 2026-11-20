using System.ComponentModel.DataAnnotations;

namespace TAJAN_TRAVEL.Models.Entities
{
    public class ActivityType
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string NameEn { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string NameFr { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? IconClass { get; set; }

        public ICollection<Tour> Tours { get; set; } = new List<Tour>();

        // ---- culture aware (English / French) - used by the public views ----
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Name => TAJAN_TRAVEL.Localization.Lang.Pick(NameEn, NameFr);
    }
}
