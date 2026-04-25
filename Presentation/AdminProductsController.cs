using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared;
using Shared.ErrorModels;
using Shared.ProductModels;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.RateLimiting;

namespace Presentation
{
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("admin")]
    public class AdminProductsController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public AdminProductsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductResultDTO), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<ProductResultDTO>> CreateProduct([FromBody] CreateProductRequestDTO productDto)
        {
            var product = await _serviceManager.ProductService.CreateProductAsync(productDto);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductResultDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProductResultDTO>> UpdateProduct(int id, [FromBody] UpdateProductRequestDTO productDto)
        {
            var product = await _serviceManager.ProductService.UpdateProductAsync(id, productDto);
            return Ok(product);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            await _serviceManager.ProductService.DeleteProductAsync(id);
            return NoContent();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ProductResultDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProductResultDTO>> GetProductById(int id)
        {
            var product = await _serviceManager.ProductService.GetProductByIdAsync(id);
            return Ok(product);
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PaginatedResult<ProductResultDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedResult<ProductResultDTO>>> GetAllProducts([FromQuery] ProductSpecificationsParameters parameters)
        {
            var products = await _serviceManager.ProductService.GetAllProductsAsync(parameters);
            return Ok(products);
        }
    }
}
