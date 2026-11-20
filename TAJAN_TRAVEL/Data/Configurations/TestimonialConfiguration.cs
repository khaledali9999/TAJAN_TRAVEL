using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Data.Configurations
{
    public class TestimonialConfiguration : IEntityTypeConfiguration<Testimonial>
    {
        public void Configure(EntityTypeBuilder<Testimonial> builder)
        {
            builder.HasOne(t => t.Tour)
                .WithMany()
                .HasForeignKey(t => t.TourId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
