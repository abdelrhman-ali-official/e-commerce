using Domain.Entities.SecurityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ProductEntities
{
    public class ProductRating : BaseEntity<int>
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        public required string UserId { get; set; }
        public User User { get; set; } = null!;
        
        public int Rating { get; set; } // 1-5 stars
        public string? Review { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
