using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ProductModels
{
    public record ProductRatingDTO
    {
        public int Id { get; init; }
        public int ProductId { get; init; }
        public string UserId { get; init; }
        public string UserName { get; init; }
        public int Rating { get; init; }
        public string? Review { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
