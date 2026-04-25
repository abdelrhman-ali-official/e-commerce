using AutoMapper;
using Domain.Entities.OrderEntities;
using Domain.Entities.PaymentEntities;
using Shared.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MappingProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(d => d.Color, options => options.MapFrom(s => s.Color.ToString()))
                .ForMember(d => d.Size, options => options.MapFrom(s => s.Size.ToString()))
                .ForMember(d => d.Subtotal, options => options.MapFrom(s => s.Price * s.Quantity));

            CreateMap<Order, OrderResultDTO>()
                .ForMember(d => d.Status, options => options.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.PaymentMethod, options => options.MapFrom(s => s.PaymentMethod.ToString()))
                .ForMember(d => d.PaymentStatus, options => options.MapFrom(s => s.PaymentStatus.ToString()))
                .ForMember(d => d.PaymentProof, options => options.MapFrom(s => s.PaymentProof));

            CreateMap<OrderPaymentProof, PaymentProofDTO>();

            CreateMap<GovernorateShippingPrice, GovernorateShippingDTO>();
        }
    }
}
