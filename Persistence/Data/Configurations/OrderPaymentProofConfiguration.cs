using Domain.Entities.PaymentEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class OrderPaymentProofConfiguration : IEntityTypeConfiguration<OrderPaymentProof>
    {
        public void Configure(EntityTypeBuilder<OrderPaymentProof> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.PayerPhone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(p => p.UploadedByUserId)
                .HasMaxLength(450);

            builder.Property(p => p.ApprovedByAdminId)
                .HasMaxLength(450);

            builder.Property(p => p.RejectedByAdminId)
                .HasMaxLength(450);

            builder.Property(p => p.RejectionReason)
                .HasMaxLength(500);

            builder.HasOne(p => p.Order)
                .WithOne(o => o.PaymentProof)
                .HasForeignKey<OrderPaymentProof>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.OrderId);
            builder.HasIndex(p => p.UploadedAt);
        }
    }
}
