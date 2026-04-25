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
    public class BasketConfiguration : IEntityTypeConfiguration<Basket>
    {
        public void Configure(EntityTypeBuilder<Basket> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(b => b.UserId)
                .HasMaxLength(450); // Match AspNetUsers Id length

            builder.HasMany(b => b.Items)
                .WithOne(i => i.Basket)
                .HasForeignKey(i => i.BasketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(b => b.CreatedAt)
                .IsRequired();

            builder.Property(b => b.UpdatedAt)
                .IsRequired();

            // Ignore calculated property
            builder.Ignore(b => b.TotalPrice);

            // Index on UserId for fast lookups
            builder.HasIndex(b => b.UserId);
        }
    }
}
