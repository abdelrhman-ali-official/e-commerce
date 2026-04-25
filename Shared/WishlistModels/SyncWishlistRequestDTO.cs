using System.Collections.Generic;

namespace Shared.WishlistModels
{
    public record SyncWishlistRequestDTO
    {
        public List<int> ProductIds { get; set; } = new();
    }
}
