using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Data.Configurations
{
    public class BlogCategoryConfiguration : IEntityTypeConfiguration<BlogCategory>
    {
        public void Configure(EntityTypeBuilder<BlogCategory> builder)
        {
            // Existing categories keep showing as filter chips after the migration
            builder.Property(c => c.ShowInFilter).HasDefaultValue(true);
        }
    }
}
