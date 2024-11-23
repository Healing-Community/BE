using Application.Commands.CreateCertificateType;
using Application.Commands.DeleteCertificateType;
using Application.Commands.UpdateCertificateType;
using Application.Queries.GetAllCertificateTypes;
using Application.Queries.GetCertificateType;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_ExpertService_API.Extentions;

namespace PRH_ExpertService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateTypeController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "Admin,Moderate")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCertificateTypes()
        {
            var response = await sender.Send(new GetAllCertificateTypesQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin,Moderate")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateCertificateType([FromBody] CreateCertificateTypeCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin,Moderate")]
        [HttpGet("get/{certificateTypeId}")]
        public async Task<IActionResult> GetCertificateType(string certificateTypeId)
        {
            var response = await sender.Send(new GetCertificateTypeQuery(certificateTypeId));
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin,Moderate")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCertificateType([FromBody] UpdateCertificateTypeCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin,Moderate")]
        [HttpDelete("delete/{certificateTypeId}")]
        public async Task<IActionResult> DeleteCertificateType(string certificateTypeId)
        {
            var response = await sender.Send(new DeleteCertificateTypeCommand(certificateTypeId));
            return response.ToActionResult();
        }
    }
}
