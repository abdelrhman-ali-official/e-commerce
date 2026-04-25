using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OrderModels
{
    public record OrderResultDTO
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? OrderToken { get; set; }
        public required string CustomerName { get; set; }
        public required string CustomerEmail { get; set; }
        public required string CustomerPhone { get; set; }
        public required string ShippingAddress { get; set; }
        public required string Governorate { get; set; }
        public string? TrackingNumber { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalPrice { get; set; }
        public required string Status { get; set; }
        public required string PaymentMethod { get; set; }
        public required string PaymentStatus { get; set; }
        public PaymentProofDTO? PaymentProof { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
    }

    public record PaymentProofDTO
    {
        public string FileUrl { get; set; } = string.Empty;
        public string PayerPhone { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedByAdminId { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? RejectedByAdminId { get; set; }
        public string? RejectionReason { get; set; }
    }
}
