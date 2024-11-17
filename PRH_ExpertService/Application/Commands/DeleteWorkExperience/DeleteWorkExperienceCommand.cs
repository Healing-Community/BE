using Application.Commons;
using MediatR;

namespace Application.Commands.DeleteWorkExperience
{
    public record DeleteWorkExperienceCommand(string WorkExperienceId) : IRequest<BaseResponse<bool>>;
}