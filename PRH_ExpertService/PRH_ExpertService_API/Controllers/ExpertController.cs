using Application.Commands.BookAppointment;
using Application.Commands.CreateAvailability;
using Application.Commands.UploadFile;
using Application.Queries.GetAvailability;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_ExpertService_API.Extentions;
using PRH_ExpertService_API.FileUpload;

namespace PRH_ExpertService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExpertController(ISender sender) : ControllerBase
{
    [Authorize(Roles = "Expert")]
    [HttpPost("upload-file/{expertId}")]
    public async Task<IActionResult> UploadFile(string expertId, [FromForm] FileUploadModel model, string certificationTypeId)
    {
        var response = await sender.Send(new UploadFileCommand(expertId, model.File, certificationTypeId));
        return response.ToActionResult();
    }

    [Authorize(Roles = "Expert")]
    [HttpPost("create-availability")]
    public async Task<IActionResult> CreateAvailability([FromBody] CreateAvailabilityCommand command)
    {
        var response = await sender.Send(command);
        return response.ToActionResult();
    }

    [Authorize]
    [HttpGet("availability/{expertProfileId}")]
    public async Task<IActionResult> GetAvailability(string expertProfileId)
    {
        var response = await sender.Send(new GetAvailabilityQuery(expertProfileId));
        return response.ToActionResult();
    }

    [Authorize(Roles = "User")]
    [HttpPost("book-appointment")]
    public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentCommand command)
    {
        var response = await sender.Send(command);
        return response.ToActionResult();
    }
}
