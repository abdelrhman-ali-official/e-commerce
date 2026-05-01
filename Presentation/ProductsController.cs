using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared;
using Shared.ProductModels;
using System.Net;
using Microsoft.AspNetCore.RateLimiting;

namespace Presentation
{
    [EnableRateLimiting("products")]
    public class ProductsController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public ProductsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "id" })]
        [ProducesResponseType(typeof(ProductResultDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ProductResultDTO>> GetProductById(int id)
        {
            var product = await _serviceManager.ProductService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            
            return Ok(product);
        }

        /// <summary>
        /// Get product by slug (SEO-friendly URL)
        /// Example: /api/products/white-cotton-tshirt-xs
        /// </summary>
        [HttpGet("{slug}")]
        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "slug" })]
        [ProducesResponseType(typeof(ProductResultDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ProductResultDTO>> GetProductBySlug(string slug)
        {
            var product = await _serviceManager.ProductService.GetProductBySlugAsync(slug);
            if (product == null)
                return NotFound(new { message = $"Product with slug '{slug}' not found" });
            
            return Ok(product);
        }

        /// <summary>
        /// Get all products with filters and pagination
        /// </summary>
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "*" })]
        [ProducesResponseType(typeof(PaginatedResult<ProductResultDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedResult<ProductResultDTO>>> GetAllProducts([FromQuery] ProductSpecificationsParameters parameters)
        {
            var products = await _serviceManager.ProductService.GetAllProductsAsync(parameters);
            return Ok(products);
        }

        [HttpGet("categories")]
        [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(IEnumerable<CategoryResultDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CategoryResultDTO>>> GetCategories()
        {
            var categories = await _serviceManager.ProductService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("brands")]
        [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(IEnumerable<BrandResultDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<BrandResultDTO>>> GetBrands()
        {
            var brands = await _serviceManager.ProductService.GetAllBrandsAsync();
            return Ok(brands);
        }

        /// <summary>
        /// Get product structured data for Schema.org (JSON-LD)
        /// Used by frontend for SEO rich snippets
        /// </summary>
        [HttpGet("{id:int}/structured-data")]
        [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "id" })]
        [ProducesResponseType(typeof(ProductStructuredDataDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ProductStructuredDataDTO>> GetProductStructuredData(int id)
        {
            var structuredData = await _serviceManager.ProductService.GetProductStructuredDataAsync(id);
            if (structuredData == null)
                return NotFound();
            
            return Ok(structuredData);
        }
    }
}
