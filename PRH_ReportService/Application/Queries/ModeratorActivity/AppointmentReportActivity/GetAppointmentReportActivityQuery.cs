using System;
using Application.Commons;
using Domain.Entities.ModeratorActivity;
using MediatR;

namespace Application.Queries.ModeratorActivity.AppointmentReportActivity;

public record GetAppointmentReportActivityQuery : IRequest<BaseResponse<IEnumerable<ModerateApointmentReportActivity>>>;