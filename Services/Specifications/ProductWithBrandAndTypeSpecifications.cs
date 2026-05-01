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
    public class ProductWithBrandAndTypeSpecifications : Specifications<Product>
    {
        public ProductWithBrandAndTypeSpecifications(int id) : base(product => product.Id == id)
        {
            AddInclude("ProductRatings.User");
            AddInclude(p => p.Category);
        }

        public ProductWithBrandAndTypeSpecifications(string slug) : base(product => product.Slug == slug)
        {
            AddInclude("ProductRatings.User");
            AddInclude(p => p.Category);
        }
        
        public ProductWithBrandAndTypeSpecifications(ProductSpecificationsParameters parameters)
            : base(product =>
            (string.IsNullOrWhiteSpace(parameters.Search) || product.Name.ToLower().Contains(parameters.Search.ToLower().Trim())) &&
            (!parameters.CategoryId.HasValue || product.CategoryId == parameters.CategoryId.Value))
        {
            AddInclude("ProductRatings.User");
            AddInclude(p => p.Category);

            ApplyPagination(parameters.PageIndex, parameters.PageSize);

            if (parameters.Sort is not null)
            {
                switch (parameters.Sort)
                {
                    case ProductSortingOptions.NameDesc:
                        setOrderByDescending(product => product.Name);
                        break;
                    case ProductSortingOptions.NameAsc:
                        setOrderBy(product => product.Name);
                        break;

                    case ProductSortingOptions.PriceDesc:
                        setOrderByDescending(product => product.Price);
                        break;
                    case ProductSortingOptions.PriceAsc:
                        setOrderBy(product => product.Price);
                        break;

                    default:
                        break;

                }
            }
        }
    }
}
