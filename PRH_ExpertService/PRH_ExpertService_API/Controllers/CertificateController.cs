﻿using Application.Commands.UploadFile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_ExpertService_API.Extentions;
using PRH_ExpertService_API.FileUpload;

namespace PRH_ExpertService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "Expert")]
        [HttpPost("upload/{expertId}")]
        public async Task<IActionResult> UploadFile(string expertId, [FromForm] FileUploadModel model, string certificationTypeId)
        {
            var response = await sender.Send(new UploadFileCommand(expertId, model.File, certificationTypeId));
            return response.ToActionResult();
        }
    }
}