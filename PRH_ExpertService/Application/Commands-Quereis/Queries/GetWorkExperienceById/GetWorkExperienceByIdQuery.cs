using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.GetWorkExperienceById
{
    public record GetWorkExperienceByIdQuery(string WorkExperienceId) : IRequest<BaseResponse<WorkExperience>>;
}
