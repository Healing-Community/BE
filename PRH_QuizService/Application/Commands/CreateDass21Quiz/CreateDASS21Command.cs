using Application.Commons;
using Domain.Entities.DASS21;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.CreateDass21Quiz
{
    public record CreateDASS21Command(Dass21 Dass21) : IRequest<BaseResponse<bool>>;
}
