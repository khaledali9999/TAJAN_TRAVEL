using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tour> Tours => Set<Tour>();
        public DbSet<TourImage> TourImages => Set<TourImage>();
        public DbSet<TourItinerary> TourItineraries => Set<TourItinerary>();
        public DbSet<TourPrice> TourPrices => Set<TourPrice>();
        public DbSet<Destination> Destinations => Set<Destination>();
        public DbSet<ActivityType> ActivityTypes => Set<ActivityType>();
        public DbSet<Inquiry> Inquiries => Set<Inquiry>();
        public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
        public DbSet<BlogCategory> BlogCategories => Set<BlogCategory>();
        public DbSet<Testimonial> Testimonials => Set<Testimonial>();
        public DbSet<FAQ> FAQs => Set<FAQ>();
        public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
        public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
        public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
        public DbSet<TravelEvent> TravelEvents => Set<TravelEvent>();
        public DbSet<EventCategory> EventCategories => Set<EventCategory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}