using Application.Commons;
using Domain.Entities;
using MediatR;

public record GetAppointmentByPropertyQuery(string AppointmentId) : IRequest<BaseResponse<Appointment>>;
