using Application.Commands.DeleteCertificate;
using Application.Commands.UpdateCertificate;
using Application.Commands.UploadCertificate;
using Application.Queries.GetAllCertificates;
using Application.Queries.GetCertificate;
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
        [Authorize(Roles = "Admin,Expert")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCertificates()
        {
            var response = await sender.Send(new GetAllCertificatesQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpPost("upload/{expertId}")]
        public async Task<IActionResult> UploadCertificate(string expertId, [FromForm] FileUploadModel model, string certificationTypeId)
        {
            var response = await sender.Send(new UploadCertificateCommand(expertId, model.File, certificationTypeId));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User,Expert")]
        [HttpGet("{certificateId}")]
        public async Task<IActionResult> GetCertificate([FromRoute] string certificateId)
        {
            var response = await sender.Send(new GetCertificateQuery(certificateId));
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
    }
}
