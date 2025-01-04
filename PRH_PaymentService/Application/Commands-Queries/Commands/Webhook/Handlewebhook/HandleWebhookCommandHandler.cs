using System.Text.RegularExpressions;
using Application.Commons;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Enum;
using ExpertPaymentService;
using MediatR;

namespace Application.Commands_Queries.Commands.Webhook.Handlewebhook;

public class HandleWebhookCommandHandler(IGrpcHelper grpcHelper, IPaymentRepository paymentRepository) : IRequestHandler<HandleWebhookCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(HandleWebhookCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var webhookData = request.WebhookRequest.Data[0];
            // Sử dụng Regular Expression để tách chuỗi vd:EXPERT7439234892 -> 7439234892 và EXPERT
            string pattern = @"([a-zA-Z]+)(\d+)";
            Match match = Regex.Match(webhookData.Description, pattern);
            if (!match.Success)
            {
                return BaseResponse<string>.BadRequest("Invalid description");
            }
            string role = match.Groups[1].Value; // Phần định danh role (EXPERT hoặc USER)
            string orderCode = match.Groups[2].Value; // Phần OrderCode

            var paymentInDb = await paymentRepository.GetByPropertyAsync(p => p.OrderCode == long.Parse(orderCode));
            if (paymentInDb == null)
            {
                return BaseResponse<string>.NotFound("Payment not found");
            }

            if (role == "EXPERT")
            {
                // Gửi appointmentId để update status của appointment về đã hủy
                var UpdateAppointmentReply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsRequest, UpdateResponse>(
                    "ExpertServiceUrl",
                    async client => await client.UpdateAppointmentAsync(new GetAppointmentsRequest { AppointmentId = paymentInDb.AppointmentId, Status = 3 })
                );
                paymentInDb.Status = (int)PaymentStatus.Completed;
                //paymentInDb.PaymentDetail = "Đã thanh toán tiền thành công cho Chuyên gia";
            }
            else if (role == "USER")
            {
                paymentInDb.Status = (int)PaymentStatus.Refunded;
                //paymentInDb.PaymentDetail = "Đã hoàn tiền cho người dùng";
            }
            await paymentRepository.Update(paymentInDb.PaymentId, paymentInDb);

            return BaseResponse<string>.SuccessReturn("Cập nhật trạng thái thanh toán thành công");
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
