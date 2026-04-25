using System;

namespace Domain.Entities.OrderEntities
{
    public class GovernorateShippingPrice : BaseEntity<int>
    {
        public required string GovernorateName { get; set; }
        public decimal ShippingPrice { get; set; }
        public int DeliveryDays { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
