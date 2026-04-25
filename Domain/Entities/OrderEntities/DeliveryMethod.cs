using System;

namespace Domain.Entities.OrderEntities
{
    public class DeliveryMethod : BaseEntity<int>
    {
        public required string ShortName { get; set; }
        public required string Description { get; set; }
        public required string DeliveryTime { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
