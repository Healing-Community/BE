﻿using Application.Commons;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.ReactionTypes.GetReactionTypesById
{
    public record GetReactionTypesByIdQuery(Guid Id) : IRequest<BaseResponse<ReactionType>>;
}
