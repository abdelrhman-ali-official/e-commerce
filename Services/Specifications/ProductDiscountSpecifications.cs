// This file is obsolete - ProductDiscount entity no longer exists
// ProductDiscount functionality is now part of Product.DiscountPercentage property
// Delete this file or keep it commented out for reference

/*
using Domain.Contracts;
using Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Specifications
{
    public class ProductDiscountSpecifications : Specifications<ProductDiscount>
    {
        public ProductDiscountSpecifications(int productId, bool onlyActive = true) 
            : base(d => d.ProductId == productId && (!onlyActive || d.IsActive))
        {
        }
    }
}
*/
