using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BasketModels
{
    public record BasketItemDTO
    {
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string ProductPictureUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public required string Color { get; set; }
        public required string Size { get; set; }
        public decimal Subtotal { get; set; }
    }
}
