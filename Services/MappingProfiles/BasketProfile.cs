using AutoMapper;
using Domain.Entities.BasketEntities;
using Shared.BasketModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MappingProfiles
{
    public class BasketProfile : Profile
    {
        public BasketProfile()
        {
            CreateMap<BasketItem, BasketItemDTO>()
                .ForMember(d => d.ProductName, options => options.MapFrom(s => s.Product.Name))
                .ForMember(d => d.ProductPictureUrl, options => options.MapFrom(s => s.Product.PictureUrl))
                .ForMember(d => d.Color, options => options.MapFrom(s => s.Color.ToString()))
                .ForMember(d => d.Size, options => options.MapFrom(s => s.Size.ToString()))
                .ForMember(d => d.Subtotal, options => options.MapFrom(s => s.Price * s.Quantity));

            CreateMap<Basket, BasketDTO>()
                .ForMember(d => d.BasketId, options => options.MapFrom(s => s.Id))
                .ForMember(d => d.TotalItems, options => options.MapFrom(s => s.Items.Sum(i => i.Quantity)));
        }
    }
}
