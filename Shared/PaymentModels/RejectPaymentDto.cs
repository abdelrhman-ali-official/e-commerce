using System.ComponentModel.DataAnnotations;

namespace Shared.PaymentModels
{
    public class RejectPaymentDto
    {
        [StringLength(500, ErrorMessage = "Rejection reason cannot exceed 500 characters")]
        public string? RejectionReason { get; set; }
    }
}
