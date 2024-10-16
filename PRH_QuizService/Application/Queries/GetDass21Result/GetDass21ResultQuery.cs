using Application.Commons;
using Domain.Entities.DASS21;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.GetDass21Result
{
    public record GetDass21ResultQuery(HttpContext HttpContext) : IRequest<BaseResponse<Dass21Result>>;
}
