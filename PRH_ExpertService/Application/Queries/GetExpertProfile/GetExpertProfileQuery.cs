using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.GetExpertProfile
{
    public record GetExpertProfileQuery(string ExpertProfileId) : IRequest<BaseResponse<ExpertProfile>>;
}