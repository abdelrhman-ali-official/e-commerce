using System.ComponentModel.DataAnnotations;

namespace Shared.OrderModels
{
    public record UpdateGovernorateShippingDTO
    {
        [Required]
        [MaxLength(100)]
        public required string GovernorateName { get; init; }

        [Required]
        [Range(0, 10000)]
        public decimal ShippingPrice { get; init; }

        [Required]
        [Range(1, 30)]
        public int DeliveryDays { get; init; }

        public bool IsActive { get; init; } = true;
    }
}
