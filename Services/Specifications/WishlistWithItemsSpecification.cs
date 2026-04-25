using Domain.Contracts;
using Domain.Entities.WishlistEntities;
using System.Linq;

namespace Services.Specifications
{
    public class WishlistWithItemsSpecification : Specifications<Wishlist>
    {
        // Get wishlist by UserId with items and products
        public WishlistWithItemsSpecification(string userId)
            : base(w => w.UserId == userId)
        {
            AddInclude(w => w.Items);
            AddInclude("Items.Product");
        }

        // Get wishlist by Id with items and products
        public WishlistWithItemsSpecification(int wishlistId, bool byId)
            : base(w => w.Id == wishlistId)
        {
            AddInclude(w => w.Items);
            AddInclude("Items.Product");
        }
    }
}
