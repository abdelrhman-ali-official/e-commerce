using Domain.Entities.ProductEntities;
using Shared.BasketModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IBasketService
    {
        Task<BasketDTO> GetBasketAsync(string? userId, string? basketId);
        Task<BasketDTO> AddItemToBasketAsync(string? userId, string? basketId, AddToBasketRequestDTO itemDto);
        Task<BasketDTO> UpdateBasketItemAsync(string? userId, string? basketId, int itemId, UpdateBasketItemRequestDTO updateDto);
        Task DeleteBasketItemAsync(string? userId, string? basketId, int productId, ProductColor color, ProductSize size);
        Task ClearBasketAsync(string? userId, string? basketId);
        Task<BasketDTO> MergeGuestBasketAsync(string userId, string guestBasketId);
    }
}
