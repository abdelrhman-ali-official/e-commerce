using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.WishlistModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Presentation
{
    [Route("api/wishlist")]
    [Authorize]
    public class WishlistController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public WishlistController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyWishlist()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var wishlist = await _serviceManager.WishlistService.GetUserWishlistAsync(userId);
            return Ok(wishlist);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistRequestDTO request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var wishlist = await _serviceManager.WishlistService.AddProductToWishlistAsync(userId, request.ProductId);
            return Ok(wishlist);
        }

        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await _serviceManager.WishlistService.RemoveProductFromWishlistAsync(userId, productId);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> ClearWishlist()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await _serviceManager.WishlistService.ClearWishlistAsync(userId);
            return NoContent();
        }

        [HttpGet("items/{productId}/check")]
        public async Task<IActionResult> IsProductInWishlist(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isInWishlist = await _serviceManager.WishlistService.IsProductInWishlistAsync(userId, productId);
            return Ok(new { isInWishlist });
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncGuestWishlist([FromBody] SyncWishlistRequestDTO request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var wishlist = await _serviceManager.WishlistService.SyncGuestWishlistAsync(userId, request.ProductIds);
            return Ok(wishlist);
        }
    }
}
