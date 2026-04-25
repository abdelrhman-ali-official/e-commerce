using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.PaymentModels;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.RateLimiting;

namespace Presentation
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/payments")]
    [EnableRateLimiting("admin")]
    public class AdminPaymentsController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public AdminPaymentsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        /// <summary>
        /// Approve a payment (Admin only)
        /// </summary>
        [HttpPost("orders/{orderId}/approve")]
        public async Task<IActionResult> ApprovePayment(int orderId)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _serviceManager.PaymentService.ApprovePaymentAsync(orderId, adminId!);
            return Ok(new { message = "Payment approved successfully" });
        }

        /// <summary>
        /// Reject a payment (Admin only)
        /// </summary>
        [HttpPost("orders/{orderId}/reject")]
        public async Task<IActionResult> RejectPayment(int orderId, [FromBody] RejectPaymentDto dto)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _serviceManager.PaymentService.RejectPaymentAsync(orderId, adminId!, dto.RejectionReason);
            return Ok(new { message = "Payment rejected successfully" });
        }

        /// <summary>
        /// Get payment proof with full details (Admin only)
        /// </summary>
        [HttpGet("orders/{orderId}/proof")]
        public async Task<IActionResult> GetPaymentProofAdmin(int orderId)
        {
            var proof = await _serviceManager.PaymentService.GetPaymentProofAsync(orderId, isAdmin: true);

            if (proof == null)
                return NotFound("No payment proof found for this order");

            return Ok(proof);
        }

        /// <summary>
        /// Update payment method settings (Admin only)
        /// </summary>
        [HttpPut("settings/{method}")]
        public async Task<IActionResult> UpdatePaymentSettings(string method, [FromBody] UpdatePaymentSettingsDto dto)
        {
            await _serviceManager.PaymentService.UpdatePaymentSettingAsync(method, dto.PhoneNumber);
            return Ok(new { message = "Payment settings updated successfully" });
        }

        /// <summary>
        /// Delete a payment proof (Admin only)
        /// </summary>
        [HttpDelete("proofs/{proofId}")]
        public async Task<IActionResult> DeletePaymentProof(int proofId)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _serviceManager.PaymentService.DeletePaymentProofAsync(proofId, adminId!);
            return NoContent();
        }
    }
}
