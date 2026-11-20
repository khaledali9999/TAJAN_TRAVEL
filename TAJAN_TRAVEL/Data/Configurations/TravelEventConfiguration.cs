using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Data.Configurations
{
    public class TravelEventConfiguration : IEntityTypeConfiguration<TravelEvent>
    {
        public void Configure(EntityTypeBuilder<TravelEvent> builder)
        {
            builder.HasIndex(e => e.Slug).IsUnique();

            builder.HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.EventCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
