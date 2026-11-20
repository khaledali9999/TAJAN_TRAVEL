using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Models.ViewModels
{
    public class TourDetailsViewModel
    {
        public Tour Tour { get; set; } = null!;
        public List<Tour> RelatedTours { get; set; } = new();
    }
}
