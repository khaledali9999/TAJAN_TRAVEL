using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Models.ViewModels
{
    // Body of POST /Wishlist/Toggle
    public class WishlistToggleRequest
    {
        public WishlistItemType Type { get; set; }
        public int RefId { get; set; }
    }

    // One saved item, already shaped for the wishlist popup (no view logic needed client-side).
    public class WishlistItemViewModel
    {
        public int Id { get; set; }              // WishlistItem.Id (used to remove this exact row)
        public string Key { get; set; } = "";     // "tour-5" / "dest-2" — used to sync heart buttons
        public string Badge { get; set; } = "";
        public string Title { get; set; } = "";
        public string Meta { get; set; } = "";
        public string Price { get; set; } = "";
        public string Image { get; set; } = "";
        public string Url { get; set; } = "";
        public string LinkLabel { get; set; } = "";
    }

    // Response shape for every Wishlist endpoint, so the client always has
    // everything it needs to repaint the popup + the header badge in one go.
    public class WishlistResultViewModel
    {
        public int Count { get; set; }
        public List<WishlistItemViewModel> Items { get; set; } = new();
    }
}
