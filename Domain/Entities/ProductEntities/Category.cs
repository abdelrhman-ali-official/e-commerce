namespace Domain.Entities.ProductEntities
{
    public class Category : BaseEntity<int>
    {
        public required string Name { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
