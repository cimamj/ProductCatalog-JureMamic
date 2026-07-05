using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasIndex(p => p.ExternalId).IsUnique();
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");

            // AI assisted: Claude helped write this
            builder.Property(p => p.Images).HasConversion(
                images => string.Join(';', images),
                serialized => serialized.Length == 0
                    ? new List<string>()
                    : serialized.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList(),
                new ValueComparer<List<string>>(
                    (a, b) => (a ?? new()).SequenceEqual(b ?? new()),
                    images => images.Aggregate(0, (hash, image) => HashCode.Combine(hash, image.GetHashCode())),
                    images => images.ToList()));

            builder.HasIndex(p => p.Price);

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
