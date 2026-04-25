using Domain.Entities.PaymentEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class PaymentSettingsConfiguration : IEntityTypeConfiguration<PaymentSettings>
    {
        public void Configure(EntityTypeBuilder<PaymentSettings> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Method)
                .IsRequired();

            builder.Property(s => s.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(s => s.DisplayName)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(s => s.Method)
                .IsUnique();
        }
    }
}
