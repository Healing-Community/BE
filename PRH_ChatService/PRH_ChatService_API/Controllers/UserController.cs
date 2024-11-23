using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PRH_ChatService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(ISender sender) : ControllerBase
{
}