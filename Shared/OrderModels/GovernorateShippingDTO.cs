namespace Shared.OrderModels
{
    public record GovernorateShippingDTO
    {
        public int Id { get; set; }
        public string GovernorateName { get; set; } = string.Empty;
        public decimal ShippingPrice { get; set; }
        public int DeliveryDays { get; set; }
        public bool IsActive { get; set; }
    }
}
