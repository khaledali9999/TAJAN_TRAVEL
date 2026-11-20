using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Data.Configurations
{
    public class InquiryConfiguration : IEntityTypeConfiguration<Inquiry>
    {
        public void Configure(EntityTypeBuilder<Inquiry> builder)
        {
            builder.HasOne(i => i.Tour)
                .WithMany(t => t.Inquiries)
                .HasForeignKey(i => i.TourId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(i => i.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(i => i.Type)
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(i => i.DateFlexibility)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(TripDateFlexibility.Exact);

            builder.Property(i => i.PreferredContact)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(InquiryContactMethod.Email);

            // Existing rows (e.g. seeded ones) get 1 adult instead of 0
            builder.Property(i => i.Adults).HasDefaultValue(1);
        }
    }
}