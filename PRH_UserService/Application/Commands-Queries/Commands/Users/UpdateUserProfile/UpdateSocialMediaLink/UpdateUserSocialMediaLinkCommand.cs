using System;
using Application.Commons;
using Application.Commons.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.UpdateUserProfile.UpdateSocialMediaLink;

public record UpdateUserSocialMediaLinkCommand(IList<SocialLinkDto> SocialLinkDtos) : IRequest<BaseResponse<string>>;