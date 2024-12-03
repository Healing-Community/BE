using Application.Commands_Queries.Queries.Roles.GetRoles;
namespace PRH_UserService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController(ISender sender) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet("gets")]
    public async Task<IActionResult> Get()
    {
        var response = await sender.Send(new GetRolesQuery());
        return response.ToActionResult();
    }
}