using System;
using Application.Commons;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using ExpertPaymentService;
using MediatR;

namespace Application.Commands_Queries.Queries.GetPaymentDetails;

public class GetPaymentDetailsQueryHandler(IGrpcHelper grpcHelper, IPaymentRepository paymentRepository) : IRequestHandler<GetPaymentDetailsQuery, BaseResponse<PaymentDetailsDTO>>
{
    public async Task<BaseResponse<PaymentDetailsDTO>> Handle(GetPaymentDetailsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var payment = await paymentRepository.GetByIdAsync(request.PaymentId);
            if (payment == null)
            {
                return BaseResponse<PaymentDetailsDTO>.NotFound();
            }
            // Lấy thông tin Appointment từ AppointmentService dùng grpc
            var appointmentDataReply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsRequest, GetAppointmentsResponse>(
                    async client => await client.GetAppointmentsAsync(new GetAppointmentsRequest { AppointmentId = payment.AppointmentId })
                );
            if (appointmentDataReply == null)
            {
                return BaseResponse<PaymentDetailsDTO>.NotFound("Lịch hẹn không tồn tại.");
            }
            //End

            var paymentDetails = new PaymentDetailsDTO
            {
                // Payment
                PaymentId = payment.PaymentId,
                AppointmentId = payment.AppointmentId,
                Amount = payment.Amount,
                // Appointment
                ExpertName = appointmentDataReply.ExpertName,
                ExpertEmail = appointmentDataReply.ExpertEmail,
                AppointmentDate = appointmentDataReply.AppointmentDate,
                StartTime = appointmentDataReply.StartTime,
                EndTime = appointmentDataReply.EndTime
            };

            return BaseResponse<PaymentDetailsDTO>.SuccessReturn(paymentDetails);
        }
        catch (Exception e)
        {
            return BaseResponse<PaymentDetailsDTO>.InternalServerError(e.Message);
            throw;
        }
    }
}
