using AutoMapper;
using Domain.Contracts;
using Domain.Entities.ProductEntities;
using Domain.Entities.WishlistEntities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Services.Abstractions;
using Services.Specifications;
using Shared.WishlistModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;

        public WishlistService(IUnitOFWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<WishlistResultDTO> GetUserWishlistAsync(string userId)
        {
            var spec = new WishlistWithItemsSpecification(userId);
            var wishlist = await _unitOfWork.GetRepository<Wishlist, int>().GetAsync(spec);

            if (wishlist == null)
            {
                // Create new wishlist if doesn't exist
                wishlist = new Wishlist
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.GetRepository<Wishlist, int>().AddAsync(wishlist);
                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.Map<WishlistResultDTO>(wishlist);
        }

        public async Task<WishlistResultDTO> AddProductToWishlistAsync(string userId, int productId)
        {
            // Verify product exists
            var product = await _unitOfWork.GetRepository<Product, int>().GetAsync(productId);
            if (product == null)
                throw new ProductNotFoundException(productId.ToString());

            // Get or create wishlist
            var spec = new WishlistWithItemsSpecification(userId);
            var wishlist = await _unitOfWork.GetRepository<Wishlist, int>().GetAsync(spec);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.GetRepository<Wishlist, int>().AddAsync(wishlist);
                await _unitOfWork.SaveChangesAsync();

                // Reload with spec to include navigation properties
                wishlist = await _unitOfWork.GetRepository<Wishlist, int>().GetAsync(spec);
            }

            // Check if product already in wishlist
            if (wishlist!.Items.Any(i => i.ProductId == productId))
            {
                throw new ValidationException(new[] { "Product is already in wishlist" });
            }

            // Add item to wishlist
            var wishlistItem = new WishlistItem
            {
                WishlistId = wishlist.Id,
                ProductId = productId,
                AddedAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<WishlistItem, int>().AddAsync(wishlistItem);
            wishlist.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            // Reload wishlist with updated items
            var updatedSpec = new WishlistWithItemsSpecification(userId);
            wishlist = await _unitOfWork.GetRepository<Wishlist, int>().GetAsync(updatedSpec);

            return _mapper.Map<WishlistResultDTO>(wishlist);
        }

        public async Task RemoveProductFromWishlistAsync(string userId, int productId)
        {
            var spec = new WishlistWithItemsSpecification(userId);
            var wishlist = await _unitOfWork.GetRepository<Wishlist, int>().GetAsync(spec);

            if (wishlist == null)
                throw new WishlistNotFoundException(userId);

            var item = wishlist.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                throw new WishlistItemNotFoundException(productId);

            _unitOfWork.GetRepository<WishlistItem, int>().Delete(item);
            wishlist.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ClearWishlistAsync(string userId)
        {
            var spec = new WishlistWithItemsSpecification(userId);
            var wishlist = await _unitOfWork.GetRepository<Wishlist, int>().GetAsync(spec);

            if (wishlist == null)
                throw new WishlistNotFoundException(userId);

            foreach (var item in wishlist.Items.ToList())
            {
                _unitOfWork.GetRepository<WishlistItem, int>().Delete(item);
            }

            wishlist.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsProductInWishlistAsync(string userId, int productId)
        {
            var spec = new WishlistWithItemsSpecification(userId);
            var wishlist = await _unitOfWork.GetRepository<Wishlist, int>().GetAsync(spec);

            if (wishlist == null)
                return false;

            return wishlist.Items.Any(i => i.ProductId == productId);
        }

        public async Task<WishlistResultDTO> SyncGuestWishlistAsync(string userId, List<int> productIds)
        {
            // Get or create user wishlist
            var spec = new WishlistWithItemsSpecification(userId);
            var wishlist = await _unitOfWork.GetRepository<Wishlist, int>().GetAsync(spec);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.GetRepository<Wishlist, int>().AddAsync(wishlist);
                await _unitOfWork.SaveChangesAsync();

                // Reload with spec
                wishlist = await _unitOfWork.GetRepository<Wishlist, int>().GetAsync(spec);
            }

            // Get existing product IDs in wishlist
            var existingProductIds = wishlist!.Items.Select(i => i.ProductId).ToHashSet();

            // Add new products that aren't already in wishlist
            foreach (var productId in productIds.Distinct())
            {
                if (!existingProductIds.Contains(productId))
                {
                    // Verify product exists
                    var product = await _unitOfWork.GetRepository<Product, int>().GetAsync(productId);
                    if (product != null)
                    {
                        var wishlistItem = new WishlistItem
                        {
                            WishlistId = wishlist.Id,
                            ProductId = productId,
                            AddedAt = DateTime.UtcNow
                        };
                        await _unitOfWork.GetRepository<WishlistItem, int>().AddAsync(wishlistItem);
                    }
                }
            }

            wishlist.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            // Reload with updated items
            var updatedSpec = new WishlistWithItemsSpecification(userId);
            wishlist = await _unitOfWork.GetRepository<Wishlist, int>().GetAsync(updatedSpec);

            return _mapper.Map<WishlistResultDTO>(wishlist);
        }
    }
}
