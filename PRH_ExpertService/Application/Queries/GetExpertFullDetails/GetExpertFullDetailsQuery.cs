using MediatR;
using Application.Commons.DTOs;
using Application.Commons;

namespace Application.Queries.GetExpertFullDetails
{
    public record GetExpertFullDetailsQuery() : IRequest<BaseResponse<ExpertFullDetailsDTO>>;
}
