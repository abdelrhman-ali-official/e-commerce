using Domain.Contracts;
using Domain.Entities.ProductEntities;
using Shared.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Specifications
{
    public class ProductCountSpecifications : Specifications<Product>
    {
        public ProductCountSpecifications(ProductSpecificationsParameters parameters)
            : base(product =>
            (string.IsNullOrWhiteSpace(parameters.Search) || product.Name.ToLower().Contains(parameters.Search.ToLower().Trim())) &&
            (!parameters.CategoryId.HasValue || product.CategoryId == parameters.CategoryId.Value))
        {
        }
    }
}
