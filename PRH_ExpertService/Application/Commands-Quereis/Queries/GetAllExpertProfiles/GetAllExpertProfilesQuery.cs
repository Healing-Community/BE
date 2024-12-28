using Application.Commons;
using Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace Application.Queries.GetAllExpertProfiles
{
    public record GetAllExpertProfilesQuery : IRequest<BaseResponse<IEnumerable<ExpertProfile>>>;
}
