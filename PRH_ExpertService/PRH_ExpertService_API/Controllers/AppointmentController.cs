﻿using Application.Commands.BookAppointment;
using Application.Commands.CancelAppointment;
using Application.Commands.DeleteAppointment;
using Application.Commands.RateExpert;
using Application.Commands.UpdateAppointment;
using Application.Commons.Tools;
using Application.Queries.GetActivityReport;
using Application.Queries.GetAllAppointments;
using Application.Queries.GetAppointmentRatingStatus;
using Application.Queries.GetAppointments;
using Application.Queries.GetAppointmentsByExpert;
using Application.Queries.GetAppointmentsByUser;
using Application.Queries.GetExpertStatistics;
using Application.Queries.GetRecentRatings;
using Application.Queries.GetUserRatingAndComment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_ExpertService_API.Extentions;

namespace PRH_ExpertService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController(ISender sender) : ControllerBase
    {
        [HttpGet("get-by-id/{appointmentId}")]
        public async Task<IActionResult> GetAppointment(string appointmentId)
        {
            var response = await sender.Send(new GetAppointmentByPropertyQuery(appointmentId));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpGet("user")]
        public async Task<IActionResult> GetAppointmentsByUser()
        {
            var userId = Authentication.GetUserIdFromHttpContext(HttpContext);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Không xác định được người dùng.");

            var response = await sender.Send(new GetAppointmentsByUserQuery(userId));
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpGet("expert")]
        public async Task<IActionResult> GetAppointmentsByExpert()
        {
            var expertProfileId = Authentication.GetUserIdFromHttpContext(HttpContext);
            if (string.IsNullOrEmpty(expertProfileId))
                return Unauthorized("Không xác định được chuyên gia.");

            var response = await sender.Send(new GetAppointmentsByExpertQuery(expertProfileId));
            return response.ToActionResult();
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var response = await sender.Send(new GetAllAppointmentsQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpPost("book")]
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin,User,Expert")]
        [HttpGet("get/{expertProfileId}")]
        public async Task<IActionResult> GetAppointments(string expertProfileId)
        {
            var response = await sender.Send(new GetAppointmentsQuery(expertProfileId));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User,Expert")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAppointment([FromBody] UpdateAppointmentCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize(Roles = "User,Expert")]
        [HttpDelete("delete/{appointmentId}")]
        public async Task<IActionResult> DeleteAppointment([FromRoute] string appointmentId)
        {
            var response = await sender.Send(new DeleteAppointmentCommand(appointmentId));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User,Expert")]
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelAppointment([FromBody] CancelAppointmentCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpPost("rate-expert")]
        public async Task<IActionResult> RateExpert([FromBody] RateExpertCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin,User,Expert")]
        [HttpGet("get-expert-ratings/{expertProfileId}")]
        public async Task<IActionResult> GetExpertRatings(string expertProfileId)
        {
            var response = await sender.Send(new GetExpertRatingsQuery(expertProfileId));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpGet("rating-status/{appointmentId}")]
        public async Task<IActionResult> GetAppointmentRatingStatus(string appointmentId)
        {
            var response = await sender.Send(new GetAppointmentRatingStatusQuery { AppointmentId = appointmentId });
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpGet("statistics")]
        public async Task<IActionResult> GetExpertStatistics()
        {
            var response = await sender.Send(new GetExpertStatisticsQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpGet("recent-ratings")]
        public async Task<IActionResult> GetRecentRatings()
        {
            var response = await sender.Send(new GetRecentRatingsQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpGet("activity-report")]
        public async Task<IActionResult> GetActivityReport()
        {
            var response = await sender.Send(new GetActivityReportQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpGet("get-rating-user/{appointmentId}")]
        public async Task<IActionResult> GetRatingUser(string appointmentId)
        {
            var response = await sender.Send(new GetUserRatingAndCommentQuery { AppointmentId = appointmentId});
            return response.ToActionResult();
        }
    }
}
