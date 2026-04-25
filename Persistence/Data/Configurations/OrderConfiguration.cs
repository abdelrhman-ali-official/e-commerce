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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.UserId)
                .HasMaxLength(450);

            builder.Property(o => o.BasketId)
                .HasMaxLength(50);

            builder.Property(o => o.OrderToken)
                .HasMaxLength(100);

            builder.Property(o => o.CustomerName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(o => o.CustomerEmail)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(o => o.CustomerPhone)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(o => o.ShippingAddress)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(o => o.Governorate)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(o => o.TrackingNumber)
                .HasMaxLength(100);

            builder.Property(o => o.SubTotal)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(o => o.ShippingCost)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(o => o.TotalPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(o => o.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(o => o.PaymentMethod)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(o => o.PaymentStatus)
                .HasConversion<int>()
                .IsRequired();

            builder.HasMany(o => o.OrderItems)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => o.UserId);
            builder.HasIndex(o => o.CreatedAt);
            builder.HasIndex(o => o.PaymentStatus);
            builder.HasIndex(o => o.OrderToken);
        }
    }
}
