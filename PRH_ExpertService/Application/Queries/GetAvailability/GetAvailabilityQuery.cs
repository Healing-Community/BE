using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.GetAvailability
{
    public record GetAvailabilityQuery(string ExpertProfileId) : IRequest<BaseResponse<IEnumerable<ExpertAvailability>>>;
}
