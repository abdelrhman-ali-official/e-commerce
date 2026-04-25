using Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BasketModels
{
    public record AddToBasketRequestDTO
    {
        [Required]
        public int ProductId { get; init; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; init; }

        [Required]
        public ProductColor Color { get; init; }

        [Required]
        public ProductSize Size { get; init; }
    }
}
