using System;

namespace Domain.Exceptions
{
    public sealed class BasketNotFoundException : NotFoundException
    {
        public BasketNotFoundException(string basketId)
            : base($"Basket with id {basketId} not found")
        {
        }
    }
}
