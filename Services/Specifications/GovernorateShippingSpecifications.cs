using Domain.Contracts;
using Domain.Entities.OrderEntities;

namespace Services.Specifications
{
    public class GovernorateShippingSpecifications : Specifications<GovernorateShippingPrice>
    {
        // Get by governorate name
        public GovernorateShippingSpecifications(string governorateName) 
            : base(g => g.GovernorateName == governorateName && g.IsActive)
        {
        }

        // Get all governorates
        public GovernorateShippingSpecifications() 
            : base(g => true)
        {
        }
    }
}
