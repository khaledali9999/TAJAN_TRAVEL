using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Data.Configurations
{
    public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
    {
        public void Configure(EntityTypeBuilder<BlogPost> builder)
        {
            builder.HasIndex(b => b.Slug).IsUnique();

            builder.Property(b => b.ReadTimeMinutes).HasDefaultValue(5);

            builder.HasOne(b => b.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(b => b.BlogCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}