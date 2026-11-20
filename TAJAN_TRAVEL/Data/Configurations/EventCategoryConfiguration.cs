using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Data.Configurations
{
    public class EventCategoryConfiguration : IEntityTypeConfiguration<EventCategory>
    {
        public void Configure(EntityTypeBuilder<EventCategory> builder)
        {
            builder.HasIndex(c => c.Slug).IsUnique();
        }
    }
}
