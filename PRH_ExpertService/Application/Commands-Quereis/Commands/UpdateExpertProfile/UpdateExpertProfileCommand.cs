using Application.Commons;
using MediatR;

namespace Application.Commands.UpdateExpertProfile
{
    public record UpdateExpertProfileCommand(string Specialization, string ExpertiseAreas, string Bio, string ProfileImageUrl, string Fullname) : IRequest<DetailBaseResponse<bool>>;
}
