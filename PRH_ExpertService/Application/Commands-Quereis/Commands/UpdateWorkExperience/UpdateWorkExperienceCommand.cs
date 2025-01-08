using Application.Commons;
using MediatR;

namespace Application.Commands.UpdateWorkExperience
{
    public record UpdateWorkExperienceCommand(string WorkExperienceId, string CompanyName, string PositionTitle, DateOnly StartDate, DateOnly EndDate, string Description) : IRequest<BaseResponse<bool>>;
}