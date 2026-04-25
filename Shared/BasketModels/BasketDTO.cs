using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BasketModels
{
    public class BasketDTO
    {
        public required string BasketId { get; set; }
        public string? UserId { get; set; }
        public List<BasketItemDTO> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public int TotalItems { get; set; }
    }
}
