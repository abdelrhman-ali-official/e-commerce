using Domain.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class GovernorateShippingPriceConfiguration : IEntityTypeConfiguration<GovernorateShippingPrice>
    {
        public void Configure(EntityTypeBuilder<GovernorateShippingPrice> builder)
        {
            builder.ToTable("GovernorateShippingPrices");

            builder.HasKey(g => g.Id);

            builder.Property(g => g.GovernorateName)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(g => g.GovernorateName)
                .IsUnique();

            builder.Property(g => g.ShippingPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(g => g.DeliveryDays)
                .IsRequired();

            builder.Property(g => g.IsActive)
                .IsRequired();
        }
    }
}
