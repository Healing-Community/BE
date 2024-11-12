using Application.Commons;
using Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace Application.Queries.GetAllExpertAvailabilities
{
    public record GetAllExpertAvailabilitiesQuery : IRequest<BaseResponse<IEnumerable<ExpertAvailability>>>;
}
