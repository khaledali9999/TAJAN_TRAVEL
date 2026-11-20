using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Models.ViewModels
{
    public class EventsViewModel
    {
        public TravelEvent? Featured { get; set; }
        public List<TravelEvent> Events { get; set; } = new();
        public List<EventCategory> Categories { get; set; } = new();

        // Active filters
        public string? Category { get; set; }
        public bool ThisMonth { get; set; }
        public bool IsFiltered => !string.IsNullOrWhiteSpace(Category) || ThisMonth;

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
    }
}
