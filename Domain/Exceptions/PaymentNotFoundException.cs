using System;

namespace Domain.Exceptions
{
    public sealed class PaymentNotFoundException : NotFoundException
    {
        public PaymentNotFoundException(string entityName, string entityId)
            : base($"{entityName} with id {entityId} not found")
        {
        }
    }
}
