using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Models.ViewModels
{
    public class TourFilterViewModel
    {
        public int? DestinationId { get; set; }
        public int? ActivityTypeId { get; set; }
        public TourType? Type { get; set; }
        public int? MinDuration { get; set; }
        public int? MaxDuration { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Duration checkboxes: "1" (day trips), "1-3", "4-7", "8-14", "15-plus"
        public List<string> DurationRanges { get; set; } = new();

        // Single-select values coming from the home page search bar: "1-3", "4-7", "8-14", "15+" / "0-500", "500-1000", "1000-2000", "2000+"
        public string? Duration { get; set; }
        public string? Price { get; set; }

        public List<Destination> Destinations { get; set; } = new();
        public List<ActivityType> ActivityTypes { get; set; } = new();
    }
}