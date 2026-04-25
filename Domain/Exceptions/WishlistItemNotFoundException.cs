namespace Domain.Exceptions
{
    public class WishlistItemNotFoundException : NotFoundException
    {
        public WishlistItemNotFoundException(int productId)
            : base($"Product with ID {productId} not found in wishlist")
        {
        }
    }
}
