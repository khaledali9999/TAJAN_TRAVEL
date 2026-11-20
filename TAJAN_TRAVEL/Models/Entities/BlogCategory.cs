using System.ComponentModel.DataAnnotations;

namespace TAJAN_TRAVEL.Models.Entities
{
    public class BlogCategory
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string NameEn { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string NameFr { get; set; } = string.Empty;

        [MaxLength(150)]
        public string Slug { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        // false = not shown as a filter chip on the Stories page (e.g. "Travel Journal")
        public bool ShowInFilter { get; set; } = true;

        public ICollection<BlogPost> Posts { get; set; } = new List<BlogPost>();

        // ---- culture aware (English / French) - used by the public views ----
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Name => TAJAN_TRAVEL.Localization.Lang.Pick(NameEn, NameFr);
    }
}