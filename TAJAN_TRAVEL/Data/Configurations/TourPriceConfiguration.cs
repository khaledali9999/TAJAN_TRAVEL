using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Data.Configurations
{
    public class TourPriceConfiguration : IEntityTypeConfiguration<TourPrice>
    {
        public void Configure(EntityTypeBuilder<TourPrice> builder)
        {
            // one price per group size per tour
            builder.HasIndex(p => new { p.TourId, p.Persons }).IsUnique();
        }
    }
}
