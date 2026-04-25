using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.BasketEntities
{
    public class Basket : BaseEntity<string>
    {
        public string? UserId { get; set; } // Nullable - null for guest baskets
        public ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Calculated property
        public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);
    }
}
