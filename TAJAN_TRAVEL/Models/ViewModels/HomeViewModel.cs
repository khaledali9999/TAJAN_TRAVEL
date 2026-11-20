using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Tour> FeaturedTours { get; set; } = new();
        public List<Testimonial> Testimonials { get; set; } = new();
        public List<Destination> FeaturedDestinations { get; set; } = new();
        public List<BlogPost> LatestBlogPosts { get; set; } = new();
        public List<FAQ> Faqs { get; set; } = new();
        public List<ActivityType> ActivityTypes { get; set; } = new();
        public List<TravelEvent> LatestEvents { get; set; } = new();
        public List<Destination> SearchDestinations { get; set; } = new();
    }
}