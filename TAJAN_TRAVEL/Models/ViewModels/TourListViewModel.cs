using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Models.ViewModels
{
    public class TourListViewModel
    {
        public List<Tour> Tours { get; set; } = new();
        public TourFilterViewModel Filter { get; set; } = new();

        // Optional page heading/intro, changes between /Tours and /Cruises
        public string PageTitle { get; set; } = "Our Tours";
        public string PageSubtitle { get; set; } = string.Empty;

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 9;
    }
}
