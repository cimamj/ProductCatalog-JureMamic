using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasIndex(c => c.ExternalSlug).IsUnique();
            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        }
    }
}
