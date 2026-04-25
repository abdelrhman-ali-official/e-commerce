using System.ComponentModel.DataAnnotations;

namespace Shared.PaymentModels
{
    public class UpdatePaymentSettingsDto
    {
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public required string PhoneNumber { get; set; }
    }
}
