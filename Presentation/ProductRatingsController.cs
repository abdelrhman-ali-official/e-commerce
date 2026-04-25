using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.ErrorModels;
using Shared.ProductModels;
using System.Net;
using System.Security.Claims;

namespace Presentation
{
    [Authorize]
    public class ProductRatingsController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public ProductRatingsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductRatingDTO), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<ProductRatingDTO>> AddRating([FromBody] CreateProductRatingRequestDTO ratingDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();
            var rating = await _serviceManager.ProductService.AddRatingAsync(userId, ratingDto);
            return CreatedAtAction(nameof(GetRatingById), new { id = rating.Id }, rating);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductRatingDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProductRatingDTO>> UpdateRating(int id, [FromBody] UpdateProductRatingRequestDTO ratingDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();
            var rating = await _serviceManager.ProductService.UpdateRatingAsync(userId, id, ratingDto);
            return Ok(rating);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult> DeleteRating(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();
            await _serviceManager.ProductService.DeleteRatingAsync(userId, id);
            return NoContent();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ProductRatingDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProductRatingDTO>> GetRatingById(int id)
        {
            // This is a placeholder - you might want to add a specific method for this
            return Ok();
        }

        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<ProductRatingDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ProductRatingDTO>>> GetProductRatings(int productId)
        {
            var ratings = await _serviceManager.ProductService.GetProductRatingsAsync(productId);
            return Ok(ratings);
        }

        [HttpGet("product/{productId}/my-rating")]
        [ProducesResponseType(typeof(ProductRatingDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProductRatingDTO?>> GetMyRatingForProduct(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();
            var rating = await _serviceManager.ProductService.GetUserRatingForProductAsync(userId, productId);
            return Ok(rating);
        }
    }
}
