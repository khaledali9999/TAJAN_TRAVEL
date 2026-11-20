using System.ComponentModel.DataAnnotations;

namespace TAJAN_TRAVEL.Models.Entities
{
    public enum WishlistItemType
    {
        Tour,
        Destination
    }

    // One saved item for one visitor (identified by an anonymous session cookie,
    // no login required). A row points at either a Tour or a Destination,
    // never both — enforced in WishlistController and at the DB level via a check
    // constraint added in WishlistItemConfiguration.
    public class WishlistItem
    {
        public int Id { get; set; }

        [Required, MaxLength(64)]
        public string SessionId { get; set; } = string.Empty;

        public WishlistItemType ItemType { get; set; }

        public int? TourId { get; set; }
        public Tour? Tour { get; set; }

        public int? DestinationId { get; set; }
        public Destination? Destination { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
