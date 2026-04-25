using System;
using System.Collections.Generic;

namespace Domain.Entities.WishlistEntities
{
    public class Wishlist : BaseEntity<int>
    {
        public required string UserId { get; set; }
        public ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
