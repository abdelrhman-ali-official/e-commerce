using Domain.Entities.ProductEntities;
using System;

namespace Domain.Entities.WishlistEntities
{
    public class WishlistItem : BaseEntity<int>
    {
        public int WishlistId { get; set; }
        public int ProductId { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Wishlist Wishlist { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
