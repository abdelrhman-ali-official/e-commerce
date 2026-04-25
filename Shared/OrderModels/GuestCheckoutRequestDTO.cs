using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OrderModels
{
    public record GuestCheckoutRequestDTO
    {
        [Required]
        [MaxLength(200)]
        public required string CustomerName { get; init; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public required string CustomerEmail { get; init; }

        [Required]
        [Phone]
        [MaxLength(20)]
        public required string CustomerPhone { get; init; }

        [Required]
        [MaxLength(500)]
        public required string ShippingAddress { get; init; }

        [Required]
        [MaxLength(100)]
        public required string Governorate { get; init; }

        /// <summary>
        /// Payment method ID: 1=CashOnDelivery, 2=VodafoneCash, 3=EtisalatCash, 4=OrangeCash, 5=InstaPay
        /// </summary>
        [Required]
        [Range(1, 5, ErrorMessage = "Payment method must be between 1 and 5")]
        public int PaymentMethodId { get; init; }

        /// <summary>
        /// Payment proof file (required for wallet/InstaPay payments, optional for COD)
        /// </summary>
        public IFormFile? PaymentProofFile { get; init; }

        /// <summary>
        /// Payer phone number (required when PaymentProofFile is provided)
        /// </summary>
        [Phone]
        [MaxLength(20)]
        public string? PayerPhone { get; init; }
    }
}
