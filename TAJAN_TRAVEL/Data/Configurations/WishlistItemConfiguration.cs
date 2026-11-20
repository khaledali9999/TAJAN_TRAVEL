using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Data.Configurations
{
    public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
    {
        public void Configure(EntityTypeBuilder<WishlistItem> builder)
        {
            builder.Property(w => w.SessionId).IsRequired().HasMaxLength(64);

            // Fast "give me this visitor's wishlist" lookups.
            builder.HasIndex(w => w.SessionId);

            // A visitor can only save a given tour / destination once.
            builder.HasIndex(w => new { w.SessionId, w.TourId }).IsUnique().HasFilter("[TourId] IS NOT NULL");
            builder.HasIndex(w => new { w.SessionId, w.DestinationId }).IsUnique().HasFilter("[DestinationId] IS NOT NULL");

            builder.HasOne(w => w.Tour)
                .WithMany()
                .HasForeignKey(w => w.TourId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(w => w.Destination)
                .WithMany()
                .HasForeignKey(w => w.DestinationId)
                .OnDelete(DeleteBehavior.Cascade);

            // A row must reference exactly one of Tour / Destination, matching ItemType.
            builder.ToTable(t => t.HasCheckConstraint(
                "CK_WishlistItem_OneTarget",
                "([ItemType] = 0 AND [TourId] IS NOT NULL AND [DestinationId] IS NULL) OR " +
                "([ItemType] = 1 AND [DestinationId] IS NOT NULL AND [TourId] IS NULL)"));
        }
    }
}
