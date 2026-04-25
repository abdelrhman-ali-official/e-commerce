using Domain.Contracts;
using Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Specifications
{
    public class ProductRatingSpecifications : Specifications<ProductRating>
    {
        // Get all ratings for a specific product
        public ProductRatingSpecifications(int productId) 
            : base(r => r.ProductId == productId)
        {
            AddInclude(r => r.User);
            setOrderByDescending(r => r.CreatedAt);
        }

        // Get a specific rating by id
        public ProductRatingSpecifications(int id, bool byId) 
            : base(r => r.Id == id)
        {
            AddInclude(r => r.User);
            AddInclude(r => r.Product);
        }

        // Get user's rating for a specific product
        public ProductRatingSpecifications(int productId, string userId) 
            : base(r => r.ProductId == productId && r.UserId == userId)
        {
            AddInclude(r => r.User);
        }
    }
}
