﻿using Application.Commands.BookAppointment;
using Application.Commands.DeleteAppointment;
using Application.Commands.UpdateAppointment;
using Application.Commons.Tools;
using Application.Queries.GetAllAppointments;
using Application.Queries.GetAppointments;
using Application.Queries.GetAppointmentsByExpert;
using Application.Queries.GetAppointmentsByUser;
using Application.Queries.GetTransactionHistory;
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
        [Authorize(Roles = "User")]
        [HttpGet("user")]
        public async Task<IActionResult> GetAppointmentsByUser()
        {
            // Lấy UserId từ token
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
            // Lấy ExpertProfileId từ token
            var expertProfileId = Authentication.GetUserIdFromHttpContext(HttpContext);
            if (string.IsNullOrEmpty(expertProfileId))
                return Unauthorized("Không xác định được chuyên gia.");

            var response = await sender.Send(new GetAppointmentsByExpertQuery(expertProfileId));
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin,User,Expert")]
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

        [Authorize]
        [HttpGet("transaction-history")]
        public async Task<IActionResult> GetUserTransactionHistory()
        {
            var response = await sender.Send(new GetTransactionHistoryQuery());
            return response.ToActionResult();
        }

    }
}
