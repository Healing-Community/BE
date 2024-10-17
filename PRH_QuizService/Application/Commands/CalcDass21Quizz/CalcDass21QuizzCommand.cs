using Application.Commons;
using Application.Commons.DTOs;
using Domain.Entities.DASS21;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.CalcDass21Quizz
{
    public record CalcDass21QuizzCommand(Dass21QuizzResultRequest Dass21QuizzResultRequest, HttpContext HttpContext) : IRequest<BaseResponse<Dass21Result>>;

}
