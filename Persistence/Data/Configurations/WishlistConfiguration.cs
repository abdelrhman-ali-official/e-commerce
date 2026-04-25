using Domain.Entities.WishlistEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.ToTable("Wishlists");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.HasIndex(w => w.UserId)
                .IsUnique();

            builder.Property(w => w.CreatedAt)
                .IsRequired();

            builder.HasMany(w => w.Items)
                .WithOne(wi => wi.Wishlist)
                .HasForeignKey(wi => wi.WishlistId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
