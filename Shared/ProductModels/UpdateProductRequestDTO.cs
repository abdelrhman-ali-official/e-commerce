using Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ProductModels
{
    public record UpdateProductRequestDTO
    {
        [MaxLength(200)]
        public string? Name { get; init; }

        [MaxLength(2000)]
        public string? Description { get; init; }

        public decimal? Price { get; init; }

        public decimal? CostPrice { get; init; } // Admin only - cost price for profit calculation

        [Range(0, 100)]
        public decimal? DiscountPercentage { get; init; }

        public ProductColor? Color { get; init; }

        public ProductSize? Size { get; init; }

        [MaxLength(500)]
        public string? PictureUrl { get; init; }

        // SEO Fields
        [MaxLength(100)]
        public string? Slug { get; init; }

        [MaxLength(60)]
        public string? SeoTitle { get; init; }

        [MaxLength(160)]
        public string? MetaDescription { get; init; }

        [MaxLength(200)]
        public string? MetaKeywords { get; init; }

        [MaxLength(125)]
        public string? ImageAlt { get; init; }
    }
}
