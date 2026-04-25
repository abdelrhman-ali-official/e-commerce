using System;

namespace Shared.PaymentModels
{
    public class PaymentProofResultDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public required string FileUrl { get; set; }
        public required string PayerPhone { get; set; }
        public DateTime UploadedAt { get; set; }
        public string? Status { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? RejectionReason { get; set; }
    }
}
