using Application.Commands.UploadFile;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PRH_ExpertService_API.Extentions;
using PRH_ExpertService_API.FileUpload;

namespace PRH_ExpertService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExpertController(ISender sender) : ControllerBase
{
    [HttpPost("upload-file/{expertId}")]
    public async Task<IActionResult> UploadFile(string expertId, [FromForm] FileUploadModel model)
    {
        var response = await sender.Send(new UploadFileCommand(expertId, model.File));
        return response.ToActionResult();
    }
}
