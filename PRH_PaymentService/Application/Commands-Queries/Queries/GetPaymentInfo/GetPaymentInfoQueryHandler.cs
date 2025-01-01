using Application.Commons;
using Application.Interfaces.Services;
using ExpertPaymentService;
using MediatR;
using UserPaymentService;

namespace Application.Commands_Queries.Queries.GetPaymentInfo;

public class GetPaymentInfoQueryHandler(IGrpcHelper grpcHelper) : IRequestHandler<GetPaymentInfoQuery, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(GetPaymentInfoQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var GetAppointmentsReply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsRequest, GetAppointmentsResponse>(
                        "ExpertServiceUrl",
                        async client => await client.GetAppointmentsAsync(new GetAppointmentsRequest { AppointmentId = request.AppointmentId })
                    );
            // Setup grpc call to get payment info from UserPaymentService
            #region UserPaymentService gRPC
            // Gửi appointmentId để lấy thông tin thanh toán
            var PaymentInfoReply = await grpcHelper.ExecuteGrpcCallAsync<UserService.UserServiceClient, GetUserPaymentInfoRequest, GetPaymentInfoResponse>(
                        "UserServiceUrl",
                        async client => await client.GetUserPaymentInfoAsync(new GetUserPaymentInfoRequest { UserId = GetAppointmentsReply.UserId })
                    );
            //End Grpc
            #endregion
            return BaseResponse<string>.SuccessReturn(PaymentInfoReply.AccountNumber,"Lấy thông tin thanh toán thành công.");
        }
        catch (Exception ex)
        {
            return BaseResponse<string>.InternalServerError(ex.Message);
        }
    }
}
