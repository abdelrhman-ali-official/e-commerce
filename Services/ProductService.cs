using AutoMapper;
using Domain.Contracts;
using Domain.Entities.ProductEntities;
using Domain.Exceptions;
using Services.Abstractions;
using Services.Specifications;
using Services.Helpers;
using Shared;
using Shared.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    internal sealed class ProductService : IProductService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOFWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Public operations
        public async Task<PaginatedResult<ProductResultDTO>> GetAllProductsAsync(ProductSpecificationsParameters parameters)
        {
            var specifications = new ProductWithBrandAndTypeSpecifications(parameters);
            var products = await _unitOfWork.GetRepository<Product, int>().GetAllAsync(specifications);

            var countSpecifications = new ProductCountSpecifications(parameters);
            var count = await _unitOfWork.GetRepository<Product, int>().CountAsync(countSpecifications);

            var productsDto = _mapper.Map<IEnumerable<ProductResultDTO>>(products);
            return new PaginatedResult<ProductResultDTO>(parameters.PageIndex, parameters.PageSize, count, productsDto);
        }

        public async Task<ProductResultDTO?> GetProductByIdAsync(int id)
        {
            var specifications = new ProductWithBrandAndTypeSpecifications(id);
            var product = await _unitOfWork.GetRepository<Product, int>().GetAsync(specifications);

            if (product is null)
                throw new ProductNotFoundException(id.ToString());

            return _mapper.Map<ProductResultDTO>(product);
        }

        public async Task<ProductResultDTO?> GetProductBySlugAsync(string slug)
        {
            var specifications = new ProductWithBrandAndTypeSpecifications(slug);
            var product = await _unitOfWork.GetRepository<Product, int>().GetAsync(specifications);

            if (product is null)
                return null;

            return _mapper.Map<ProductResultDTO>(product);
        }

        public async Task<ProductStructuredDataDTO?> GetProductStructuredDataAsync(int id)
        {
            var product = await _unitOfWork.GetRepository<Product, int>().GetAsync(id);

            if (product is null)
                return null;

            var structuredData = new ProductStructuredDataDTO
            {
                Name = product.Name,
                Description = product.Description,
                Image = product.PictureUrl,
                Sku = $"PRD-{product.Id}",
                Offers = new ProductOfferDTO
                {
                    Url = $"/products/{product.Slug}",
                    Price = product.FinalPrice ?? product.Price,
                    PriceValidUntil = DateTime.UtcNow.AddMonths(1).ToString("yyyy-MM-dd")
                }
            };

            if (product.TotalRatings > 0)
            {
                structuredData.AggregateRating = new ProductAggregateRatingDTO
                {
                    RatingValue = product.AverageRating,
                    ReviewCount = product.TotalRatings
                };
            }

            return structuredData;
        }

        public async Task<IEnumerable<ProductResultDTO>> GetProductsByIdsAsync(IEnumerable<int> ids)
        {
            var products = new List<Product>();
            foreach (var id in ids)
            {
                var specifications = new ProductWithBrandAndTypeSpecifications(id);
                var product = await _unitOfWork.GetRepository<Product, int>().GetAsync(specifications);
                if (product is not null)
                    products.Add(product);
            }
            return _mapper.Map<IEnumerable<ProductResultDTO>>(products);
        }

        // Admin operations
        public async Task<ProductResultDTO> CreateProductAsync(CreateProductRequestDTO productDto)
        {
            await EnsureBrandExistsAsync(productDto.BrandId);
            await EnsureCategoryExistsAsync(productDto.CategoryId);

            var product = _mapper.Map<Product>(productDto);

            // Generate SEO fields if not provided
            if (string.IsNullOrWhiteSpace(product.Slug))
            {
                product.Slug = SlugHelper.GenerateProductSlug(product.Name, product.Color.ToString(), product.Size.ToString());
            }

            if (string.IsNullOrWhiteSpace(product.SeoTitle))
            {
                product.SeoTitle = $"{product.Name} - {product.Color} {product.Size}";
            }

            if (string.IsNullOrWhiteSpace(product.MetaDescription))
            {
                var desc = product.Description.Length > 155 
                    ? product.Description.Substring(0, 155) + "..." 
                    : product.Description;
                product.MetaDescription = desc;
            }

            if (string.IsNullOrWhiteSpace(product.ImageAlt))
            {
                product.ImageAlt = $"{product.Name} {product.Color} {product.Size}";
            }

            await _unitOfWork.GetRepository<Product, int>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            // Ensure unique slug with ID
            var allProducts = await _unitOfWork.GetRepository<Product, int>().GetAllAsync();
            var existingSlugs = allProducts.Where(p => p.Id != product.Id).Select(p => p.Slug).ToList();
            product.Slug = SlugHelper.EnsureUnique(product.Slug, existingSlugs);
            await _unitOfWork.SaveChangesAsync();

            return (await GetProductByIdAsync(product.Id))!;
        }

        public async Task<ProductResultDTO> UpdateProductAsync(int id, UpdateProductRequestDTO productDto)
        {
            var product = await _unitOfWork.GetRepository<Product, int>().GetAsync(id);
            if (product is null)
                throw new ProductNotFoundException(id.ToString());

            if (productDto.CategoryId.HasValue)
            {
                await EnsureCategoryExistsAsync(productDto.CategoryId.Value);
            }

            if (productDto.BrandId.HasValue)
            {
                await EnsureBrandExistsAsync(productDto.BrandId.Value);
            }

            // Update properties if provided
            if (!string.IsNullOrWhiteSpace(productDto.Name))
                product.Name = productDto.Name;

            if (productDto.Description is not null)
                product.Description = productDto.Description;

            if (productDto.Price.HasValue)
                product.Price = productDto.Price.Value;

            if (productDto.Quantity.HasValue)
                product.Quantity = productDto.Quantity.Value;

            if (productDto.BrandId.HasValue)
                product.BrandId = productDto.BrandId.Value;

            if (productDto.CategoryId.HasValue)
                product.CategoryId = productDto.CategoryId.Value;

            if (productDto.CostPrice.HasValue)
                product.CostPrice = productDto.CostPrice.Value;

            if (productDto.DiscountPercentage.HasValue)
                product.DiscountPercentage = productDto.DiscountPercentage.Value;

            if (productDto.Color.HasValue)
                product.Color = productDto.Color.Value;

            if (productDto.Size.HasValue)
                product.Size = productDto.Size.Value;

            if (productDto.PictureUrl is not null)
                product.PictureUrl = productDto.PictureUrl;

            if (productDto.Slug is not null)
                product.Slug = productDto.Slug;

            if (productDto.SeoTitle is not null)
                product.SeoTitle = productDto.SeoTitle;

            if (productDto.MetaDescription is not null)
                product.MetaDescription = productDto.MetaDescription;

            if (productDto.MetaKeywords is not null)
                product.MetaKeywords = productDto.MetaKeywords;

            if (productDto.ImageAlt is not null)
                product.ImageAlt = productDto.ImageAlt;

            _unitOfWork.GetRepository<Product, int>().Update(product);
            await _unitOfWork.SaveChangesAsync();

            return (await GetProductByIdAsync(id))!;
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.GetRepository<Product, int>().GetAsync(id);
            if (product is null)
                throw new ProductNotFoundException(id.ToString());

            _unitOfWork.GetRepository<Product, int>().Delete(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<BrandResultDTO> CreateBrandAsync(CreateBrandRequestDTO brandDto)
        {
            var brandRepository = _unitOfWork.GetRepository<Brand, int>();
            var normalizedName = brandDto.Name.Trim();

            var existingBrands = await brandRepository.GetAllAsync();
            var exists = existingBrands.Any(b => b.Name.ToLower() == normalizedName.ToLower());
            if (exists)
            {
                throw new ValidationException(new[] { $"Brand '{normalizedName}' already exists." });
            }

            var brand = new Brand { Name = normalizedName };
            await brandRepository.AddAsync(brand);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BrandResultDTO>(brand);
        }

        public async Task<IEnumerable<BrandResultDTO>> GetAllBrandsAsync()
        {
            var brands = await _unitOfWork.GetRepository<Brand, int>().GetAllAsync();
            return _mapper.Map<IEnumerable<BrandResultDTO>>(brands.OrderBy(b => b.Name));
        }

        public async Task<CategoryResultDTO> CreateCategoryAsync(CreateCategoryRequestDTO categoryDto)
        {
            var categoryRepository = _unitOfWork.GetRepository<Category, int>();
            var normalizedName = categoryDto.Name.Trim();

            var existingCategories = await categoryRepository.GetAllAsync();
            var exists = existingCategories.Any(c => c.Name.ToLower() == normalizedName.ToLower());
            if (exists)
            {
                throw new ValidationException(new[] { $"Category '{normalizedName}' already exists." });
            }

            var category = new Category { Name = normalizedName };
            await categoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CategoryResultDTO>(category);
        }

        public async Task<IEnumerable<CategoryResultDTO>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.GetRepository<Category, int>().GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryResultDTO>>(categories.OrderBy(c => c.Name));
        }

        private async Task EnsureCategoryExistsAsync(int categoryId)
        {
            var category = await _unitOfWork.GetRepository<Category, int>().GetAsync(categoryId);
            if (category is null)
            {
                throw new ValidationException(new[] { $"Category with id '{categoryId}' does not exist." });
            }
        }

        private async Task EnsureBrandExistsAsync(int brandId)
        {
            var brand = await _unitOfWork.GetRepository<Brand, int>().GetAsync(brandId);
            if (brand is null)
            {
                throw new ValidationException(new[] { $"Brand with id '{brandId}' does not exist." });
            }
        }

        // Rating operations
        public async Task<ProductRatingDTO> AddRatingAsync(string userId, CreateProductRatingRequestDTO ratingDto)
        {
            // Check if product exists
            var product = await _unitOfWork.GetRepository<Product, int>().GetAsync(ratingDto.ProductId);
            if (product is null)
                throw new ProductNotFoundException(ratingDto.ProductId.ToString());

            // Check if user already rated this product
            var existingRatingSpec = new ProductRatingSpecifications(ratingDto.ProductId, userId);
            var existingRating = await _unitOfWork.GetRepository<ProductRating, int>().GetAsync(existingRatingSpec);
            if (existingRating is not null)
                throw new ValidationException(new[] { "You have already rated this product. Use update instead." });

            var rating = new ProductRating
            {
                ProductId = ratingDto.ProductId,
                UserId = userId,
                Rating = ratingDto.Rating,
                Review = ratingDto.Review,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<ProductRating, int>().AddAsync(rating);
            await _unitOfWork.SaveChangesAsync();

            var ratingSpec = new ProductRatingSpecifications(rating.Id, true);
            var savedRating = await _unitOfWork.GetRepository<ProductRating, int>().GetAsync(ratingSpec);

            return _mapper.Map<ProductRatingDTO>(savedRating);
        }

        public async Task<ProductRatingDTO> UpdateRatingAsync(string userId, int ratingId, UpdateProductRatingRequestDTO ratingDto)
        {
            var ratingSpec = new ProductRatingSpecifications(ratingId, true);
            var rating = await _unitOfWork.GetRepository<ProductRating, int>().GetAsync(ratingSpec);

            if (rating is null)
                throw new ProductNotFoundException(ratingId.ToString());

            if (rating.UserId != userId)
                throw new UnAuthorizedException("You can only update your own ratings");

            rating.Rating = ratingDto.Rating;
            rating.Review = ratingDto.Review;
            rating.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GetRepository<ProductRating, int>().Update(rating);
            await _unitOfWork.SaveChangesAsync();

            var updatedRatingSpec = new ProductRatingSpecifications(ratingId, true);
            var updatedRating = await _unitOfWork.GetRepository<ProductRating, int>().GetAsync(updatedRatingSpec);

            return _mapper.Map<ProductRatingDTO>(updatedRating);
        }

        public async Task DeleteRatingAsync(string userId, int ratingId)
        {
            var ratingSpec = new ProductRatingSpecifications(ratingId, true);
            var rating = await _unitOfWork.GetRepository<ProductRating, int>().GetAsync(ratingSpec);

            if (rating is null)
                throw new ProductNotFoundException(ratingId.ToString());

            if (rating.UserId != userId)
                throw new UnAuthorizedException("You can only delete your own ratings");

            _unitOfWork.GetRepository<ProductRating, int>().Delete(rating);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductRatingDTO>> GetProductRatingsAsync(int productId)
        {
            var product = await _unitOfWork.GetRepository<Product, int>().GetAsync(productId);
            if (product is null)
                throw new ProductNotFoundException(productId.ToString());

            var ratingsSpec = new ProductRatingSpecifications(productId);
            var ratings = await _unitOfWork.GetRepository<ProductRating, int>().GetAllAsync(ratingsSpec);

            return _mapper.Map<IEnumerable<ProductRatingDTO>>(ratings);
        }

        public async Task<ProductRatingDTO?> GetUserRatingForProductAsync(string userId, int productId)
        {
            var product = await _unitOfWork.GetRepository<Product, int>().GetAsync(productId);
            if (product is null)
                throw new ProductNotFoundException(productId.ToString());

            var ratingSpec = new ProductRatingSpecifications(productId, userId);
            var rating = await _unitOfWork.GetRepository<ProductRating, int>().GetAsync(ratingSpec);

            return rating is null ? null : _mapper.Map<ProductRatingDTO>(rating);
        }
    }
}
