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

        [HttpPost("categories")]
        [ProducesResponseType(typeof(CategoryResultDTO), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<CategoryResultDTO>> CreateCategory([FromBody] CreateCategoryRequestDTO categoryDto)
        {
            var category = await _serviceManager.ProductService.CreateCategoryAsync(categoryDto);
            return CreatedAtAction(nameof(GetCategories), new { }, category);
        }

        [HttpPost("brands")]
        [ProducesResponseType(typeof(BrandResultDTO), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<BrandResultDTO>> CreateBrand([FromBody] CreateBrandRequestDTO brandDto)
        {
            var brand = await _serviceManager.ProductService.CreateBrandAsync(brandDto);
            return CreatedAtAction(nameof(GetBrands), new { }, brand);
        }

        [HttpGet("brands")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<BrandResultDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<BrandResultDTO>>> GetBrands()
        {
            var brands = await _serviceManager.ProductService.GetAllBrandsAsync();
            return Ok(brands);
        }

        [HttpGet("categories")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<CategoryResultDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CategoryResultDTO>>> GetCategories()
        {
            var categories = await _serviceManager.ProductService.GetAllCategoriesAsync();
            return Ok(categories);
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
