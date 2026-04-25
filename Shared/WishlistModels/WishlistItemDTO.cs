using Shared.ProductModels;
using System;

namespace Shared.WishlistModels
{
    public record WishlistItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string ProductPictureUrl { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal FinalPrice { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
