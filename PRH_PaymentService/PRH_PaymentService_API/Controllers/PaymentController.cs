using Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PRH_PaymentService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(ISender sender, IPayOSService payOSService) : ControllerBase
    {

    }
}