using Application.Commons;
using MediatR;

namespace Application.Commands.UpdateWorkExperience
{
    public record UpdateWorkExperienceCommand(string WorkExperienceId, string CompanyName, string PositionTitle, DateTime StartDate, DateTime EndDate, string Description) : IRequest<DetailBaseResponse<bool>>;
}