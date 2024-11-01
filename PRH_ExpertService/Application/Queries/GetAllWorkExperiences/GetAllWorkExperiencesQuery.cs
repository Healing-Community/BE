using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.GetAllWorkExperiences
{
    public record GetAllWorkExperiencesQuery : IRequest<BaseResponse<IEnumerable<WorkExperience>>>;
}