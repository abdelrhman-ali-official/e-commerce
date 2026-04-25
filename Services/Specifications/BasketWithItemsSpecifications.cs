using Domain.Contracts;
using Domain.Entities.BasketEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Specifications
{
    public class BasketWithItemsSpecifications : Specifications<Basket>
    {
        // Get basket by BasketId (for guests)
        public BasketWithItemsSpecifications(string basketId) 
            : base(b => b.Id == basketId)
        {
            AddInclude(b => b.Items);
            AddInclude("Items.Product");
        }

        // Get basket by UserId (for authenticated users)
        public BasketWithItemsSpecifications(string userId, bool byUserId) 
            : base(b => b.UserId == userId)
        {
            AddInclude(b => b.Items);
            AddInclude("Items.Product");
        }
    }
}
