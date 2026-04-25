using AutoMapper;
using Domain.Entities.WishlistEntities;
using Shared.WishlistModels;
using System.Linq;

namespace Services.MappingProfiles
{
    public class WishlistProfile : Profile
    {
        public WishlistProfile()
        {
            CreateMap<Wishlist, WishlistResultDTO>()
                .ForMember(d => d.TotalItems, opt => opt.MapFrom(s => s.Items.Count));

            CreateMap<WishlistItem, WishlistItemDTO>()
                .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product.Name))
                .ForMember(d => d.ProductDescription, opt => opt.MapFrom(s => s.Product.Description))
                .ForMember(d => d.ProductPictureUrl, opt => opt.MapFrom(s => s.Product.PictureUrl))
                .ForMember(d => d.ProductPrice, opt => opt.MapFrom(s => s.Product.Price))
                .ForMember(d => d.DiscountPercentage, opt => opt.MapFrom(s => s.Product.DiscountPercentage))
                .ForMember(d => d.FinalPrice, opt => opt.MapFrom(s => 
                    s.Product.DiscountPercentage.HasValue 
                        ? s.Product.Price - (s.Product.Price * s.Product.DiscountPercentage.Value / 100)
                        : s.Product.Price));
        }
    }
}
