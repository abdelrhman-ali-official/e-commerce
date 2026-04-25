using Domain.Entities.BasketEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    public class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
    {
        public void Configure(EntityTypeBuilder<BasketItem> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.BasketId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(i => i.Quantity)
                .IsRequired();

            builder.Property(i => i.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(i => i.Color)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(i => i.Size)
                .HasConversion<int>()
                .IsRequired();

            builder.HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique index to prevent duplicate items in same basket with same product/color/size
            builder.HasIndex(i => new { i.BasketId, i.ProductId, i.Color, i.Size })
                .IsUnique();
        }
    }
}
