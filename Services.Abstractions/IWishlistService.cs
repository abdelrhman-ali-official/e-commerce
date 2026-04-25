using Shared.WishlistModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IWishlistService
    {
        Task<WishlistResultDTO> GetUserWishlistAsync(string userId);
        Task<WishlistResultDTO> AddProductToWishlistAsync(string userId, int productId);
        Task RemoveProductFromWishlistAsync(string userId, int productId);
        Task ClearWishlistAsync(string userId);
        Task<bool> IsProductInWishlistAsync(string userId, int productId);
        Task<WishlistResultDTO> SyncGuestWishlistAsync(string userId, List<int> productIds);
    }
}
