using System;
using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.UserReference;

public record CreateUserReferenceCommand (UserPreferenceDto UserPreferenceDto) : IRequest<BaseResponse<string>>;
