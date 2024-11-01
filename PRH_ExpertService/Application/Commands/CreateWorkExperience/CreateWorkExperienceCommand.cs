using Application.Commons;
using MediatR;

namespace Application.Commands.CreateWorkExperience
{
    public record CreateWorkExperienceCommand(string ExpertProfileId, string CompanyName, string PositionTitle, DateTime StartDate, DateTime EndDate, string Description) : IRequest<BaseResponse<string>>;
}