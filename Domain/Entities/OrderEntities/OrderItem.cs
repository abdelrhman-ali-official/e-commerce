using Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.OrderEntities
{
    public class OrderItem : BaseEntity<int>
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string ProductPictureUrl { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; } // Price at time of order (FinalPrice after discount)
        public decimal CostPrice { get; set; } // Cost price at time of order (for profit calculation)
        public ProductColor Color { get; set; }
        public ProductSize Size { get; set; }
    }
}
