namespace Domain.Entities.PaymentEntities
{
    public class PaymentSettings : BaseEntity<int>
    {
        public PaymentMethod Method { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
