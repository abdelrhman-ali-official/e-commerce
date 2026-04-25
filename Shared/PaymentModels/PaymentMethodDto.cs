namespace Shared.PaymentModels
{
    public class PaymentMethodDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
