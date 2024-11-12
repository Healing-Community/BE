using Application.Commons;
using MediatR;

public record GetUserProfileQuery(string UserId) : IRequest<BaseResponse<UserProfileDto>>;