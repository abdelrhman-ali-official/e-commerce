using Domain.Entities.WishlistEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
    {
        public void Configure(EntityTypeBuilder<WishlistItem> builder)
        {
            builder.ToTable("WishlistItems");

            builder.HasKey(wi => wi.Id);

            builder.Property(wi => wi.WishlistId)
                .IsRequired();

            builder.Property(wi => wi.ProductId)
                .IsRequired();

            builder.Property(wi => wi.AddedAt)
                .IsRequired();

            builder.HasIndex(wi => new { wi.WishlistId, wi.ProductId })
                .IsUnique();

            builder.HasOne(wi => wi.Product)
                .WithMany()
                .HasForeignKey(wi => wi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
