using Domain.Entities.ProductEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    public class ProductRatingConfiguration : IEntityTypeConfiguration<ProductRating>
    {
        public void Configure(EntityTypeBuilder<ProductRating> builder)
        {
            builder.HasOne(r => r.Product)
                .WithMany(p => p.ProductRatings)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(r => r.Rating)
                .IsRequired();

            builder.Property(r => r.Review)
                .HasMaxLength(1000);

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            // Ensure one rating per user per product
            builder.HasIndex(r => new { r.ProductId, r.UserId })
                .IsUnique();
        }
    }
}
