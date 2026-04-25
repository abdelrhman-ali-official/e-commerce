using System.ComponentModel.DataAnnotations;

namespace Shared.OrderModels
{
    public record UpdateOrderStatusDTO
    {
        /// <summary>
        /// Order status: 1=Pending, 2=Confirmed, 3=Shipping, 4=Delivered, 5=Cancelled
        /// </summary>
        [Required]
        [Range(1, 5)]
        public int Status { get; init; }

        [MaxLength(100)]
        public string? TrackingNumber { get; init; }
    }
}
