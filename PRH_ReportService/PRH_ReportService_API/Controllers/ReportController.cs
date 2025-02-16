﻿using Application.Commands_Queries.Queries.Report.Appointment;
using Application.Commands_Queries.Queries.Report.Comment.GetCommentReport;
using Application.Commands_Queries.Queries.Report.GetPostReport;
using Application.Queries.SystemReport;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRH_ReportService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportController(ISender sender) : ControllerBase
{
    [Authorize(Roles = "Moderator")]
    [HttpGet("get-post-report")]
    public async Task<IActionResult> GetPostReports()
    {
        var response = await sender.Send(new GetPostReportsQuery());
        return response.ToActionResult();
    }
    [Authorize(Roles = "Moderator")]
    [HttpGet("get-comment-report")]
    public async Task<IActionResult> GetCommentReports()
    {
        var response = await sender.Send(new GetCommentReportQuery());
        return response.ToActionResult();
    }
    [Authorize(Roles = "Moderator")]
    [HttpGet("get-appointment-report")]
    public async Task<IActionResult> GetAppointmentReports()
    {
        var response = await sender.Send(new GetAppointmentReportQuery());
        return response.ToActionResult();
    }
    [Authorize(Roles="Admin")]
    [HttpGet("get-system-report")]
    public async Task<IActionResult> GetSystemReports()
    {
        var response = await sender.Send(new GetsUserSystemReportQuery());
        return response.ToActionResult();
    }
}
