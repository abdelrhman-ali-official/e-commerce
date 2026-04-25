using Domain.Contracts;
using Domain.Entities.OrderEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Specifications
{
    public class OrderWithItemsSpecifications : Specifications<Order>
    {
        // Get all orders
        public OrderWithItemsSpecifications()
            : base(o => true)
        {
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.PaymentProof);
            setOrderByDescending(o => o.CreatedAt);
        }

        // Get order by Id
        public OrderWithItemsSpecifications(int orderId) 
            : base(o => o.Id == orderId)
        {
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.PaymentProof);
        }

        // Get orders by UserId
        public OrderWithItemsSpecifications(string userId, bool byUserId) 
            : base(o => o.UserId == userId)
        {
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.PaymentProof);
            setOrderByDescending(o => o.CreatedAt);
        }
    }
}
