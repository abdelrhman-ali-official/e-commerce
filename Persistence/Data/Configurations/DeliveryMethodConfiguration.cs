using Domain.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class DeliveryMethodConfiguration : IEntityTypeConfiguration<DeliveryMethod>
    {
        public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
        {
            builder.ToTable("DeliveryMethods");

            builder.HasKey(dm => dm.Id);

            builder.Property(dm => dm.ShortName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(dm => dm.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(dm => dm.DeliveryTime)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(dm => dm.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(dm => dm.IsActive)
                .IsRequired();
        }
    }
}
