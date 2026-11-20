using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Models.ViewModels
{
    public class DestinationListViewModel
    {
        public List<Destination> Destinations { get; set; } = new();

        // "Egypt in a Day" slider under the grid (full page loads only)
        public List<Tour> FeaturedTours { get; set; } = new();

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 6;
    }
}
