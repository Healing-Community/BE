using Application.Commons;
using Application.Commons.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.ReactionTypes.UpdateReactionType
{
    public record UpdateReactionTypeCommand(Guid reactionTypeId, ReactionTypeDto ReactionTypeDto) : IRequest<BaseResponse<string>>;
}
