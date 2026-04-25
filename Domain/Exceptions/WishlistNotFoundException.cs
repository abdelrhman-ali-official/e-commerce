namespace Domain.Exceptions
{
    public class WishlistNotFoundException : NotFoundException
    {
        public WishlistNotFoundException(string userId)
            : base($"Wishlist for user {userId} not found")
        {
        }
    }
}
