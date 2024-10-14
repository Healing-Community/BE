using Microsoft.AspNetCore.Mvc;

namespace PRH_ReportService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("ReportController");
    }
}