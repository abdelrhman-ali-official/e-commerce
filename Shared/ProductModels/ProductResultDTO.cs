using Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ProductModels
{
    public record ProductResultDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal FinalPrice { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalRatings { get; set; }

        // SEO Fields
        public string Slug { get; set; } = string.Empty;
        public string? SeoTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public string? ImageAlt { get; set; }
        public string CanonicalUrl { get; set; } = string.Empty;
    }
}
