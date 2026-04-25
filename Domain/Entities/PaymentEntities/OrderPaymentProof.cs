using Domain.Entities.OrderEntities;
using System;

namespace Domain.Entities.PaymentEntities
{
    public class OrderPaymentProof : BaseEntity<int>
    {
        public int OrderId { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string PayerPhone { get; set; } = string.Empty;
        public string? UploadedByUserId { get; set; }
        public DateTime UploadedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedByAdminId { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? RejectedByAdminId { get; set; }
        public string? RejectionReason { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
    }
}
