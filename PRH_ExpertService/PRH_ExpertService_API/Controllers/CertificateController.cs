﻿using Application.Commands.ApproveCertificate;
using Application.Commands.DeleteCertificate;
using Application.Commands.RejectCertificate;
using Application.Commands.UpdateCertificate;
using Application.Commands.UploadCertificate;
using Application.Queries.GetAllCertificates;
using Application.Queries.GetCertificate;
using Application.Queries.GetCertificatesByExpert;
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
        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCertificates()
        {
            var response = await sender.Send(new GetAllCertificatesQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadCertificate([FromForm] FileUploadModel model, string certificationTypeId)
        {
            var response = await sender.Send(new UploadCertificateCommand(model.File, certificationTypeId));
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin,User,Expert")]
        [HttpGet("get-certificate{certificateId}")]
        public async Task<IActionResult> GetCertificate([FromRoute] string certificateId)
        {
            var response = await sender.Send(new GetCertificateQuery(certificateId));
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin,User,Expert")]
        [HttpGet("get-certificates-by-expert/{expertProfileId}")]
        public async Task<IActionResult> GetCertificatesByExpertProfileId([FromRoute] string expertProfileId)
        {
            var response = await sender.Send(new GetCertificatesByExpertQuery(expertProfileId));
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCertificate([FromBody] UpdateCertificateCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpDelete("delete/{certificateId}")]
        public async Task<IActionResult> DeleteCertificate([FromRoute] string certificateId)
        {
            var response = await sender.Send(new DeleteCertificateCommand(certificateId));
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost("approve")]
        public async Task<IActionResult> ApproveCertificate([FromBody] ApproveCertificateCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost("reject")]
        public async Task<IActionResult> RejectCertificate([FromBody] RejectCertificateCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }
    }
}
