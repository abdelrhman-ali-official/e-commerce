using Domain.Entities.ProductEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using Services.Abstractions;
using Shared.BasketModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Presentation
{
    [Route("api/cart")]
    public class BasketController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public BasketController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetBasket()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var basketId = BasketResolver.GetBasketId(HttpContext);

            var basket = await _serviceManager.BasketService.GetBasketAsync(userId, basketId);

            // Set cookie if guest basket was created
            if (string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(basket.BasketId))
            {
                BasketResolver.SetBasketId(HttpContext, basket.BasketId);
            }

            return Ok(basket);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItemToBasket([FromBody] AddToBasketRequestDTO itemDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var basketId = BasketResolver.GetBasketId(HttpContext);

            var basket = await _serviceManager.BasketService.AddItemToBasketAsync(userId, basketId, itemDto);

            // Set cookie if guest basket was created
            if (string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(basket.BasketId))
            {
                BasketResolver.SetBasketId(HttpContext, basket.BasketId);
            }

            return Ok(basket);
        }

        [HttpPut("items/{itemId}")]
        public async Task<IActionResult> UpdateBasketItem(int itemId, [FromBody] UpdateBasketItemRequestDTO updateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var basketId = BasketResolver.GetBasketId(HttpContext);

            var basket = await _serviceManager.BasketService.UpdateBasketItemAsync(userId, basketId, itemId, updateDto);

            return Ok(basket);
        }

        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> DeleteBasketItem(int productId, [FromQuery] ProductColor? color = null, [FromQuery] ProductSize? size = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var basketId = BasketResolver.GetBasketId(HttpContext);

            // If color and size are not provided, find and delete any item with this productId
            if (!color.HasValue || !size.HasValue)
            {
                // Get basket to find the item
                var basket = await _serviceManager.BasketService.GetBasketAsync(userId, basketId);
                var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
                
                if (item == null)
                    return NotFound($"Product {productId} not found in basket");
                
                // Parse the color and size from the item
                if (!Enum.TryParse<ProductColor>(item.Color, out var itemColor) || 
                    !Enum.TryParse<ProductSize>(item.Size, out var itemSize))
                {
                    return BadRequest("Invalid color or size in basket item");
                }
                
                await _serviceManager.BasketService.DeleteBasketItemAsync(userId, basketId, productId, itemColor, itemSize);
            }
            else
            {
                await _serviceManager.BasketService.DeleteBasketItemAsync(userId, basketId, productId, color.Value, size.Value);
            }

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> ClearBasket()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var basketId = BasketResolver.GetBasketId(HttpContext);

            await _serviceManager.BasketService.ClearBasketAsync(userId, basketId);

            // Clear cookie if guest
            if (string.IsNullOrEmpty(userId))
            {
                BasketResolver.ClearBasketId(HttpContext);
            }

            return NoContent();
        }

        [Authorize]
        [HttpPost("merge")]
        public async Task<IActionResult> MergeGuestBasket()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var guestBasketId = BasketResolver.GetBasketId(HttpContext);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (string.IsNullOrEmpty(guestBasketId))
                return BadRequest("No guest basket found to merge");

            var basket = await _serviceManager.BasketService.MergeGuestBasketAsync(userId, guestBasketId);

            // Clear guest basket cookie after merge
            BasketResolver.ClearBasketId(HttpContext);

            return Ok(basket);
        }
    }
}
