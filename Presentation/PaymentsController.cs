using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using Services.Abstractions;
using Shared.PaymentModels;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.RateLimiting;

namespace Presentation
{
    [Route("api/payments")]
    [EnableRateLimiting("default")]
    public class PaymentsController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public PaymentsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        /// <summary>
        /// Get available payment methods
        /// </summary>
        [HttpGet("methods")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var methods = await _serviceManager.PaymentService.GetPaymentMethodsAsync();
            return Ok(methods);
        }

        /// <summary>
        /// Upload payment proof for an order (authenticated user or guest with basketId)
        /// </summary>
        [HttpPost("orders/{orderId}/proof")]
        [EnableRateLimiting("uploads")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadPaymentProof(
            int orderId,
            [FromForm] IFormFile file,
            [FromForm] string payerPhone)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var basketId = BasketResolver.GetBasketId(HttpContext);

            var result = await _serviceManager.PaymentService.UploadPaymentProofAsync(
                orderId,
                userId,
                basketId,
                file,
                payerPhone
            );

            return Ok(result);
        }

        /// <summary>
        /// Get payment proof for an order (non-admin: redacted URL)
        /// </summary>
        [HttpGet("orders/{orderId}/proof")]
        public async Task<IActionResult> GetPaymentProof(int orderId)
        {
            var isAdmin = User.IsInRole("Admin");
            var proof = await _serviceManager.PaymentService.GetPaymentProofAsync(orderId, isAdmin);

            if (proof == null)
                return NotFound("No payment proof found for this order");

            return Ok(proof);
        }
    }
}
