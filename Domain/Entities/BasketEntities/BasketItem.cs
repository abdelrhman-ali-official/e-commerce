using Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.BasketEntities
{
    public class BasketItem : BaseEntity<int>
    {
        public required string BasketId { get; set; }
        public Basket Basket { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal Price { get; set; } // Store price at time of adding to cart
        public ProductColor Color { get; set; }
        public ProductSize Size { get; set; }
    }
}
