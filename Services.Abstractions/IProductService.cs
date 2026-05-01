global using Shared;
using Shared.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Services.Abstractions
{
    public interface IProductService
    {
        // Public operations (for all users)
        Task<PaginatedResult<ProductResultDTO>> GetAllProductsAsync(ProductSpecificationsParameters parameters);
        Task<ProductResultDTO?> GetProductByIdAsync(int id);
        Task<ProductResultDTO?> GetProductBySlugAsync(string slug);
        Task<IEnumerable<ProductResultDTO>> GetProductsByIdsAsync(IEnumerable<int> ids);
        Task<ProductStructuredDataDTO?> GetProductStructuredDataAsync(int id);

        // Admin operations
        Task<ProductResultDTO> CreateProductAsync(CreateProductRequestDTO productDto);
        Task<ProductResultDTO> UpdateProductAsync(int id, UpdateProductRequestDTO productDto);
        Task DeleteProductAsync(int id);
        Task<BrandResultDTO> CreateBrandAsync(CreateBrandRequestDTO brandDto);
        Task<IEnumerable<BrandResultDTO>> GetAllBrandsAsync();
        Task<CategoryResultDTO> CreateCategoryAsync(CreateCategoryRequestDTO categoryDto);
        Task<IEnumerable<CategoryResultDTO>> GetAllCategoriesAsync();

        // Rating operations (for authenticated users)
        Task<ProductRatingDTO> AddRatingAsync(string userId, CreateProductRatingRequestDTO ratingDto);
        Task<ProductRatingDTO> UpdateRatingAsync(string userId, int ratingId, UpdateProductRatingRequestDTO ratingDto);
        Task DeleteRatingAsync(string userId, int ratingId);
        Task<IEnumerable<ProductRatingDTO>> GetProductRatingsAsync(int productId);
        Task<ProductRatingDTO?> GetUserRatingForProductAsync(string userId, int productId);
    }
}
