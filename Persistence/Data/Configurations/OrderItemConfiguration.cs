using Domain.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.ProductName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(i => i.ProductPictureUrl)
                .HasMaxLength(500)
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
        }
    }
}
