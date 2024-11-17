using System;
using Application.Commons;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands_Queries.Commands.Users.UpdateUserProfile.UpdateProfilePicture;

public record UpdateProfilePictureCommand(IFormFile ProfilePicture) : IRequest<BaseResponse<string>>;
