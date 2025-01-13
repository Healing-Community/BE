using Application.Commands_Queries.Commands.Report;
using Application.Commands_Queries.Commands.Users.UserReportSystem;

namespace PRH_UserService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpPost("send-report")]
    public async Task<IActionResult> SendReportAsync([FromBody] ReportMessageDto reportMessageDto)
    {
        reportMessageDto.context = HttpContext;
        var response = await sender.Send(new ReportCommand(reportMessageDto));
        return response.ToActionResult();
    }
    /// <summary>
    /// User gửi báo cáo - góp ý về hệ thống 
    /// </summary>
    /// <param name="userReportSystemCommand"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("send-report-system")]
    public async Task<IActionResult> SendReportSystemAsync([FromBody] UserReportSystemCommand userReportSystemCommand)
    {
        var response = await sender.Send(userReportSystemCommand);
        return response.ToActionResult();
    }
}