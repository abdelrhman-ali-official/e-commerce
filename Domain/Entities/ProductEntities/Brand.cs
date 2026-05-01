namespace Domain.Entities.ProductEntities
{
    public class Brand : BaseEntity<int>
    {
        public required string Name { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
