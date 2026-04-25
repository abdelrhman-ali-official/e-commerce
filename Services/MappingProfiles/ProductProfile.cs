using AutoMapper;
using Domain.Entities.ProductEntities;
using Shared.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Services.MappingProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // ProductBrand and ProductType no longer exist

            CreateMap<ProductRating, ProductRatingDTO>()
                .ForMember(d => d.UserName, options => options.MapFrom(s => s.User.UserName ?? "Anonymous"));

            CreateMap<Product, ProductResultDTO>()
                .ForMember(d => d.Color, options => options.MapFrom(s => s.Color.ToString()))
                .ForMember(d => d.Size, options => options.MapFrom(s => s.Size.ToString()))
                .ForMember(d => d.FinalPrice, options => options.MapFrom(s => s.FinalPrice))
                .ForMember(d => d.AverageRating, options => options.MapFrom(s => s.AverageRating))
                .ForMember(d => d.TotalRatings, options => options.MapFrom(s => s.TotalRatings))
                .ForMember(d => d.CanonicalUrl, options => options.MapFrom(s => $"/products/{s.Slug}"));

            CreateMap<CreateProductRequestDTO, Product>();
            CreateMap<CreateProductRatingRequestDTO, ProductRating>();
        }
    }
}
