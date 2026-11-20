using Microsoft.EntityFrameworkCore;

namespace TAJAN_TRAVEL.Data.Seed
{
    public static class DbSeeder
    {
        /// <summary>
        /// Only applies pending migrations. NO data is inserted any more:
        /// everything on the site comes from what you add in the admin panel.
        /// (The old seeders - TourSeeder, DestinationSeeder, SampleDataSeeder, StorySeeder,
        /// EventSeeder, AdminSeeder - are no longer called and can be deleted.)
        /// </summary>
        public static void Seed(ApplicationDbContext context)
        {
            context.Database.Migrate();
        }
    }
}