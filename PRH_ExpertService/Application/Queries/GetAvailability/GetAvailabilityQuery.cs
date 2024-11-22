using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.GetAvailability
{
    public record GetAvailabilityQuery() : IRequest<BaseResponse<IEnumerable<ExpertAvailability>>>;
}
