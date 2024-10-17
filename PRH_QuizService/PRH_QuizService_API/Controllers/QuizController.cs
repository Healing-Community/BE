using Application.Commands;
using Application.Commands.CalcDass21Quizz;
using Application.Commands.CreateDass21Quiz;
using Application.Commons.DTOs;
using Application.Queries;
using Application.Queries.GetDass21Result;
using Application.Queries.GetDass22Quizz;
using Domain.Entities.DASS21;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRH_QuizService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuizController(ISender sender) : ControllerBase
{
    [Obsolete]
    [HttpPost("create-dass21-quizz")]
    public async Task<IActionResult> Get()
    {
        var response = await sender.Send(new CreateDASS21Command(new Dass21() { Id = NewId.NextSequentialGuid() }));
        return response.ToActionResult();
    }
    [HttpGet("get_dass21_quizz")]
    public async Task<IActionResult> GetDass21()
    {
        var response = await sender.Send(new GetDASS21Quizz());
        return response.ToActionResult();
    }
    [Authorize(Roles = "User")]
    [HttpPost("submit_dass21_quizz")]
    public async Task<IActionResult> SubmitDass21([FromBody] Dass21QuizzResultRequest dass21QuizzResultRequest)
    {
        var response = await sender.Send(new CalcDass21QuizzCommand(dass21QuizzResultRequest,HttpContext));
        return response.ToActionResult();
    }

    [Authorize(Roles = "User")]
    [HttpGet("get_dass21_result")]
    public async Task<IActionResult> GetDass21Result()
    {
        var response = await sender.Send(new GetDass21ResultQuery(HttpContext));
        return response.ToActionResult();
    }
}