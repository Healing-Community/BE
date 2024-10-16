using Application.Commons;
using Domain.Entities.DASS21;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.GetDass22Quizz
{
    public record GetDASS21Quizz : IRequest<BaseResponse<Dass21>>;
}
