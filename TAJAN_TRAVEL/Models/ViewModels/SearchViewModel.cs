using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Models.ViewModels
{
    public class SearchViewModel
    {
        public string Query { get; set; } = string.Empty;

        public List<Tour> Tours { get; set; } = new();
        public List<Tour> Cruises { get; set; } = new();
        public List<Destination> Destinations { get; set; } = new();
        public List<BlogPost> Stories { get; set; } = new();
        public List<TravelEvent> Events { get; set; } = new();
        public List<FAQ> Faqs { get; set; } = new();

        public int Total => Tours.Count + Cruises.Count + Destinations.Count + Stories.Count + Events.Count + Faqs.Count;
    }
}
