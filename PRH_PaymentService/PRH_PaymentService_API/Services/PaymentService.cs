using Application.Interfaces.Repositories;
using Grpc.Core;
using MediatR;
using PaymentExpertService;
public class PaymentService(ISender sender, IPaymentRepository paymentRepository) : PaymentExpertService.PaymentService.PaymentServiceBase
{
    public async override Task<UpdatePaymentAppointResponse> UpdatePayment(UpdatePaymentAppointmentRequest request, ServerCallContext context)
    {
        bool IsSucess = false;
        try
        {
            var appointmentId = request.AppointmentId;
            var Status = request.Status;
            var payment = await paymentRepository.GetByPropertyAsync(x => x.AppointmentId == appointmentId);
            if (payment == null)
            {
                throw new Exception("Không tìm thấy thông tin thanh toán.");
            }
            payment.Status = Status;
            await paymentRepository.Update(payment.PaymentId, payment);
            IsSucess = true;
        }
        catch
        {
            IsSucess = false;
            throw;
        }

        return new UpdatePaymentAppointResponse
        {
            IsSucess = IsSucess
        };
    }
}