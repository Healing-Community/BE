using Application.Commons;
using Application.Commons.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Queries.GetExpertProfile
{
    public record GetExpertProfileQuery(string ExpertProfileId) : IRequest<BaseResponse<ExpertProfileDTO>>;
}