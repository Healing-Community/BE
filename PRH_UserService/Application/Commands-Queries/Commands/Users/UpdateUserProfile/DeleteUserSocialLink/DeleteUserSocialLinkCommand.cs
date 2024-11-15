using System;
using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.UpdateUserProfile.DeleteUserSocialLink;

public record DeleteUserSocialLinkCommand(string[] PlatformNames) : IRequest<BaseResponse<string>>;