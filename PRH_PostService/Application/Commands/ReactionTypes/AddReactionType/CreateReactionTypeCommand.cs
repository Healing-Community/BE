using Application.Commons;
using Application.Commons.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.ReactionTypes.AddReactionType
{
    public record CreateReactionTypeCommand(ReactionTypeDto ReactionTypeDto) : IRequest<BaseResponse<string>>;
}
