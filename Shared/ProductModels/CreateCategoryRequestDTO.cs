using System.ComponentModel.DataAnnotations;

namespace Shared.ProductModels
{
    public record CreateCategoryRequestDTO
    {
        [Required]
        [MaxLength(150)]
        public required string Name { get; init; }
    }
}
