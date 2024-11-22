using Application.Commons;
using MediatR;

namespace Application.Commands.CreateWorkExperience
{
    public record CreateWorkExperienceCommand(string CompanyName, string PositionTitle, DateTime StartDate, DateTime EndDate, string Description) : IRequest<DetailBaseResponse<string>>;
}