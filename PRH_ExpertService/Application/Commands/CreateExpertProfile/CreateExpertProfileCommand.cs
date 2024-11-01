using Application.Commons;
using MediatR;

namespace Application.Commands.CreateExpertProfile
{
    public record CreateExpertProfileCommand(string Specialization, string ExpertiseAreas, string Bio) : IRequest<BaseResponse<string>>;
}
