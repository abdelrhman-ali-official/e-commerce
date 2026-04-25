using AutoMapper;
using Domain.Contracts;
using Domain.Entities.BasketEntities;
using Domain.Entities.ProductEntities;
using Domain.Exceptions;
using Services.Abstractions;
using Services.Specifications;
using Shared.BasketModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    internal sealed class BasketService : IBasketService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;

        public BasketService(IUnitOFWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BasketDTO> GetBasketAsync(string? userId, string? basketId)
        {
            Basket? basket = null;

            if (!string.IsNullOrEmpty(userId))
            {
                // Get basket by UserId for authenticated users
                var spec = new BasketWithItemsSpecifications(userId, byUserId: true);
                basket = await _unitOfWork.GetRepository<Basket, string>().GetAsync(spec);
            }
            else if (!string.IsNullOrEmpty(basketId))
            {
                // Get basket by BasketId for guests
                var spec = new BasketWithItemsSpecifications(basketId);
                basket = await _unitOfWork.GetRepository<Basket, string>().GetAsync(spec);
            }

            // If no basket exists, return empty basket
            if (basket == null)
            {
                return new BasketDTO
                {
                    BasketId = basketId ?? Guid.NewGuid().ToString(),
                    UserId = userId,
                    Items = new List<BasketItemDTO>(),
                    TotalPrice = 0,
                    TotalItems = 0
                };
            }

            return _mapper.Map<BasketDTO>(basket);
        }

        public async Task<BasketDTO> AddItemToBasketAsync(string? userId, string? basketId, AddToBasketRequestDTO itemDto)
        {
            // Validate product exists
            var product = await _unitOfWork.GetRepository<Product, int>().GetAsync(itemDto.ProductId);
            if (product == null)
                throw new ProductNotFoundException(itemDto.ProductId.ToString());

            Basket? basket = null;
            string actualBasketId = basketId ?? Guid.NewGuid().ToString();

            // Try to get existing basket
            if (!string.IsNullOrEmpty(userId))
            {
                var spec = new BasketWithItemsSpecifications(userId, byUserId: true);
                basket = await _unitOfWork.GetRepository<Basket, string>().GetAsync(spec);
            }
            else if (!string.IsNullOrEmpty(basketId))
            {
                var spec = new BasketWithItemsSpecifications(basketId);
                basket = await _unitOfWork.GetRepository<Basket, string>().GetAsync(spec);
            }

            // Create new basket if doesn't exist
            if (basket == null)
            {
                basket = new Basket
                {
                    Id = actualBasketId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.GetRepository<Basket, string>().AddAsync(basket);
                
                // Save the basket first to establish the foreign key relationship
                await _unitOfWork.SaveChangesAsync();
            }

            // Check if item with same product/color/size already exists
            var existingItem = basket.Items.FirstOrDefault(i =>
                i.ProductId == itemDto.ProductId &&
                i.Color == itemDto.Color &&
                i.Size == itemDto.Size);

            if (existingItem != null)
            {
                // Update quantity
                existingItem.Quantity += itemDto.Quantity;
                _unitOfWork.GetRepository<BasketItem, int>().Update(existingItem);
            }
            else
            {
                // Add new item
                var newItem = new BasketItem
                {
                    BasketId = basket.Id,
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    Price = product.FinalPrice ?? product.Price,
                    Color = itemDto.Color,
                    Size = itemDto.Size
                };
                await _unitOfWork.GetRepository<BasketItem, int>().AddAsync(newItem);
            }

            basket.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.GetRepository<Basket, string>().Update(basket);
            await _unitOfWork.SaveChangesAsync();

            return await GetBasketAsync(userId, basket.Id);
        }

        public async Task<BasketDTO> UpdateBasketItemAsync(string? userId, string? basketId, int itemId, UpdateBasketItemRequestDTO updateDto)
        {
            var item = await _unitOfWork.GetRepository<BasketItem, int>().GetAsync(itemId);
            if (item == null)
                throw new BasketItemNotFoundException(itemId.ToString());

            // Verify item belongs to user's basket
            var basket = await GetBasketEntityAsync(userId, basketId);
            if (basket.Id != item.BasketId)
                throw new UnAuthorizedException("This item does not belong to your basket");

            item.Quantity = updateDto.Quantity;
            _unitOfWork.GetRepository<BasketItem, int>().Update(item);

            basket.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.GetRepository<Basket, string>().Update(basket);

            await _unitOfWork.SaveChangesAsync();

            return await GetBasketAsync(userId, basketId);
        }

        public async Task DeleteBasketItemAsync(string? userId, string? basketId, int productId, ProductColor color, ProductSize size)
        {
            var basket = await GetBasketEntityAsync(userId, basketId);

            var item = basket.Items.FirstOrDefault(i =>
                i.ProductId == productId &&
                i.Color == color &&
                i.Size == size);

            if (item == null)
                throw new BasketItemNotFoundException($"Product {productId} with color {color} and size {size}");

            _unitOfWork.GetRepository<BasketItem, int>().Delete(item);

            basket.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.GetRepository<Basket, string>().Update(basket);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ClearBasketAsync(string? userId, string? basketId)
        {
            var basket = await GetBasketEntityAsync(userId, basketId);

            _unitOfWork.GetRepository<Basket, string>().Delete(basket);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<BasketDTO> MergeGuestBasketAsync(string userId, string guestBasketId)
        {
            // Get guest basket
            var guestSpec = new BasketWithItemsSpecifications(guestBasketId);
            var guestBasket = await _unitOfWork.GetRepository<Basket, string>().GetAsync(guestSpec);

            if (guestBasket == null || !guestBasket.Items.Any())
            {
                // No guest basket to merge, return user's basket
                return await GetBasketAsync(userId, null);
            }

            // Get or create user basket
            var userSpec = new BasketWithItemsSpecifications(userId, byUserId: true);
            var userBasket = await _unitOfWork.GetRepository<Basket, string>().GetAsync(userSpec);

            if (userBasket == null)
            {
                // User has no basket, assign guest basket to user
                guestBasket.UserId = userId;
                guestBasket.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.GetRepository<Basket, string>().Update(guestBasket);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<BasketDTO>(guestBasket);
            }

            // Merge items from guest basket to user basket
            foreach (var guestItem in guestBasket.Items)
            {
                var existingItem = userBasket.Items.FirstOrDefault(i =>
                    i.ProductId == guestItem.ProductId &&
                    i.Color == guestItem.Color &&
                    i.Size == guestItem.Size);

                if (existingItem != null)
                {
                    // Add quantities
                    existingItem.Quantity += guestItem.Quantity;
                    _unitOfWork.GetRepository<BasketItem, int>().Update(existingItem);
                }
                else
                {
                    // Add new item to user basket
                    var newItem = new BasketItem
                    {
                        BasketId = userBasket.Id,
                        ProductId = guestItem.ProductId,
                        Quantity = guestItem.Quantity,
                        Price = guestItem.Price,
                        Color = guestItem.Color,
                        Size = guestItem.Size
                    };
                    await _unitOfWork.GetRepository<BasketItem, int>().AddAsync(newItem);
                }
            }

            userBasket.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.GetRepository<Basket, string>().Update(userBasket);

            // Delete guest basket
            _unitOfWork.GetRepository<Basket, string>().Delete(guestBasket);

            await _unitOfWork.SaveChangesAsync();

            return await GetBasketAsync(userId, null);
        }

        private async Task<Basket> GetBasketEntityAsync(string? userId, string? basketId)
        {
            Basket? basket = null;

            if (!string.IsNullOrEmpty(userId))
            {
                var spec = new BasketWithItemsSpecifications(userId, byUserId: true);
                basket = await _unitOfWork.GetRepository<Basket, string>().GetAsync(spec);
            }
            else if (!string.IsNullOrEmpty(basketId))
            {
                var spec = new BasketWithItemsSpecifications(basketId);
                basket = await _unitOfWork.GetRepository<Basket, string>().GetAsync(spec);
            }

            if (basket == null)
                throw new BasketNotFoundException(userId ?? basketId ?? "unknown");

            return basket;
        }
    }
}
