using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ProductEntities
{
    public class Product : BaseEntity<int>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string PictureUrl { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; } // Admin only - for profit calculation
        public decimal? DiscountPercentage { get; set; } // 0-100
        public ProductColor Color { get; set; }
        public ProductSize Size { get; set; }

        // SEO Fields
        public string Slug { get; set; } = string.Empty; // URL-friendly name
        public string? SeoTitle { get; set; } // Custom SEO title (max 60 chars)
        public string? MetaDescription { get; set; } // Meta description (max 160 chars)
        public string? MetaKeywords { get; set; } // Comma-separated keywords
        public string? ImageAlt { get; set; } // Alt text for main image

        // Navigation property for ratings
        public ICollection<ProductRating> ProductRatings { get; set; } = new List<ProductRating>();

        // Calculated properties
        public decimal? FinalPrice => DiscountPercentage.HasValue 
            ? Price - (Price * DiscountPercentage.Value / 100) 
            : Price;

        public decimal AverageRating => ProductRatings.Any() 
            ? (decimal)ProductRatings.Average(r => r.Rating) 
            : 0;

        public int TotalRatings => ProductRatings.Count;
    }
}
