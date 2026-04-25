using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ProductModels
{
    public record UpdateProductRatingRequestDTO
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; init; }

        [MaxLength(1000)]
        public string? Review { get; init; }
    }
}
