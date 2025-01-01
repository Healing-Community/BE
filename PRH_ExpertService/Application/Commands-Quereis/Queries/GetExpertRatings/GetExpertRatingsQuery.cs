using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.RateExpert
{
    public record GetExpertRatingsQuery(string ExpertProfileId) : IRequest<BaseResponse<ExpertRatingsResponseDTO>>;
}
