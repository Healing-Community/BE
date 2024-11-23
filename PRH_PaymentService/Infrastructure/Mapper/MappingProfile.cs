using AutoMapper;
using Domain.Enum;

namespace Infrastructure.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<string, PaymentStatus>()
                .ConvertUsing(status => MapPayOSStatusToPaymentStatus(status));
        }

        private static PaymentStatus MapPayOSStatusToPaymentStatus(string payOSStatus)
        {
            return payOSStatus switch
            {
                "PENDING" => PaymentStatus.Pending,
                "PAID" => PaymentStatus.Paid,
                "FAILED" => PaymentStatus.Failed,
                "CANCELLED" => PaymentStatus.Cancelled,
                _ => PaymentStatus.Unknown
            };
        }
    }
}
