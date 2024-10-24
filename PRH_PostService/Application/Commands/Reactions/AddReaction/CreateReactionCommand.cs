﻿using Application.Commons;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Reactions.AddReaction
{
    public record CreateReactionCommand(ReactionDto ReactionDto, HttpContext HttpContext) : IRequest<BaseResponse<string>>;
}
