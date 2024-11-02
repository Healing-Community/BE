using Application.Commons;
using MediatR;

namespace Application.Commands.UpdateExpertProfile
{
    public record UpdateExpertProfileCommand(string ExpertProfileId, string? Specialization, string? ExpertiseAreas, string? Bio, int? Status) : IRequest<DetailBaseResponse<bool>>;
}
