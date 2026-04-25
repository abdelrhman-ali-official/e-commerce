using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using Services.Abstractions;
using Shared.OrderModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Presentation
{
    [Route("api/orders")]
    public class OrdersController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public OrdersController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost("guest-checkout")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> GuestCheckout([FromForm] GuestCheckoutRequestDTO checkoutDto)
        {
            var basketId = BasketResolver.GetBasketId(HttpContext);

            if (string.IsNullOrEmpty(basketId))
                return BadRequest("No basket found");

            var order = await _serviceManager.OrderService.CreateGuestOrderAsync(basketId, checkoutDto);

            // Clear basket cookie after order
            BasketResolver.ClearBasketId(HttpContext);

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [Authorize]
        [HttpPost("checkout")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UserCheckout([FromForm] GuestCheckoutRequestDTO checkoutDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var order = await _serviceManager.OrderService.CreateUserOrderAsync(userId, checkoutDto);

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _serviceManager.OrderService.GetOrderByIdAsync(id);
            return Ok(order);
        }

        [Authorize]
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var orders = await _serviceManager.OrderService.GetUserOrdersAsync(userId);

            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/all")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _serviceManager.OrderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("admin/{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDTO dto)
        {
            var order = await _serviceManager.OrderService.UpdateOrderStatusAsync(orderId, dto.Status, dto.TrackingNumber);
            return Ok(order);
        }

        [HttpGet("shipping/governorates")]
        public async Task<IActionResult> GetAvailableGovernoratesForShipping()
        {
            var governorates = await _serviceManager.OrderService.GetAllGovernorateShippingAsync();
            return Ok(governorates);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/shipping/governorates")]
        public async Task<IActionResult> GetGovernorateShipping()
        {
            var governorates = await _serviceManager.OrderService.GetAllGovernorateShippingAsync();
            return Ok(governorates);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("admin/shipping/governorates/{id}")]
        public async Task<IActionResult> UpdateGovernorateShipping(int id, [FromBody] UpdateGovernorateShippingDTO dto)
        {
            var governorate = await _serviceManager.OrderService.UpdateGovernorateShippingAsync(id, dto);
            return Ok(governorate);
        }
    }
}
