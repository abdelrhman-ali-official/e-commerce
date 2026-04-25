using System;

namespace Domain.Exceptions
{
    public sealed class BasketItemNotFoundException : NotFoundException
    {
        public BasketItemNotFoundException(string itemId)
            : base($"Basket item with id {itemId} not found")
        {
        }
    }
}
