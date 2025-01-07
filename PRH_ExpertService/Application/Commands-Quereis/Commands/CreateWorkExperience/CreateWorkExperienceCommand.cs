using Application.Commons;
using MediatR;

namespace Application.Commands.CreateWorkExperience
{
    public record CreateWorkExperienceCommand(string CompanyName, string PositionTitle, DateOnly StartDate, DateOnly EndDate, string Description) : IRequest<BaseResponse<string>>;
}