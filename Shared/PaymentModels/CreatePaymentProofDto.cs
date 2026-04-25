using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Shared.PaymentModels
{
    public class CreatePaymentProofDto
    {
        [Required(ErrorMessage = "Payment proof file is required")]
        public required IFormFile File { get; set; }

        [Required(ErrorMessage = "Payer phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public required string PayerPhone { get; set; }
    }
}
