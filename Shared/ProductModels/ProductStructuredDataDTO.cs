namespace Shared.ProductModels
{
    /// <summary>
    /// Product structured data for Schema.org (JSON-LD)
    /// Used for rich snippets in search results
    /// </summary>
    public class ProductStructuredDataDTO
    {
        public string Context { get; set; } = "https://schema.org/";
        public string Type { get; set; } = "Product";
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string Brand { get; set; } = "YourStore";
        public ProductOfferDTO Offers { get; set; } = new();
        public ProductAggregateRatingDTO? AggregateRating { get; set; }
    }

    public class ProductOfferDTO
    {
        public string Type { get; set; } = "Offer";
        public string Url { get; set; } = string.Empty;
        public string PriceCurrency { get; set; } = "EGP";
        public decimal Price { get; set; }
        public string Availability { get; set; } = "https://schema.org/InStock";
        public string PriceValidUntil { get; set; } = string.Empty;
    }

    public class ProductAggregateRatingDTO
    {
        public string Type { get; set; } = "AggregateRating";
        public decimal RatingValue { get; set; }
        public int ReviewCount { get; set; }
        public int BestRating { get; set; } = 5;
        public int WorstRating { get; set; } = 1;
    }
}
