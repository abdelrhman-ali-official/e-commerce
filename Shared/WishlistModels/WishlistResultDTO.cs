using System;
using System.Collections.Generic;

namespace Shared.WishlistModels
{
    public record WishlistResultDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public List<WishlistItemDTO> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
