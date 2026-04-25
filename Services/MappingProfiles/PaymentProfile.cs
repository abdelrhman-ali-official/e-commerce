using AutoMapper;
using Domain.Entities.PaymentEntities;
using Shared.PaymentModels;

namespace Services.MappingProfiles
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<OrderPaymentProof, PaymentProofResultDto>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s =>
                    s.ApprovedAt.HasValue ? "Approved" :
                    s.RejectedAt.HasValue ? "Rejected" : "Pending"));
        }
    }
}
