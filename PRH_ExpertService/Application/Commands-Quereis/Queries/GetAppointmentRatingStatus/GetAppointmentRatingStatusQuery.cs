using MediatR;
using Application.Commons;

namespace Application.Queries.GetAppointmentRatingStatus
{
    public class GetAppointmentRatingStatusQuery : IRequest<BaseResponse<bool>>
    {
        public string AppointmentId { get; set; }
    }
}
