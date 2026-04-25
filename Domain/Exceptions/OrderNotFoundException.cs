using System;

namespace Domain.Exceptions
{
    public sealed class OrderNotFoundException : NotFoundException
    {
        public OrderNotFoundException(string orderId)
            : base($"Order with id {orderId} not found")
        {
        }
    }
}
