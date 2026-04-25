using Domain.Entities.PaymentEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.OrderEntities
{
    public class Order : BaseEntity<int>
    {
        public string? UserId { get; set; } // Nullable for guest orders
        public string? BasketId { get; set; } // Track which basket was used
        public string? OrderToken { get; set; } // Token for guest order access

        public required string CustomerName { get; set; }
        public required string CustomerEmail { get; set; }
        public required string CustomerPhone { get; set; }
        public required string ShippingAddress { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalPrice { get; set; }
        
        // Shipping fields
        public required string Governorate { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        // Payment fields
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public int? PaymentProofId { get; set; }
        public OrderPaymentProof? PaymentProof { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public enum OrderStatus
    {
        Pending = 1,
        Confirmed = 2,
        Shipping = 3,
        Delivered = 4,
        Cancelled = 5
    }
}
