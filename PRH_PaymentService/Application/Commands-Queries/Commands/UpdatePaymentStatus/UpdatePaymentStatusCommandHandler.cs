using Application.Commons;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using ExpertPaymentService;
using MediatR;
using UserPaymentService;

namespace Application.Commands.CancelPaymentLink
{
    public class UpdatePaymentStatusCommandHandler(IPlatformFeeRepository platformFeeRepository,IGrpcHelper grpcHelper, IPaymentRepository paymentRepository) : IRequestHandler<UpdatePaymentStatusCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var payment = await paymentRepository.GetByOrderCodeAsync(request.OrderCode);
                if (payment == null)
                {
                    return BaseResponse<string>.NotFound("Không tìm thấy thông tin thanh toán.");
                }
                if (request.Status == (int)Domain.Enum.PaymentStatus.Cancelled)
                {
                    // Update payment status to cancelled
                    await paymentRepository.UpdateStatus(request.OrderCode, Domain.Enum.PaymentStatus.Cancelled);
                    // Grpc qua expert để lấy thông tin lịch hẹn đồng thời kiểm tra xem lịch hẹn có tồn tại không
                    // Gửi appointmentId qua expert-service để update status của appointment và expertAvailability về đã hủy

                    // Gửi appointmentId để update status của appointment về đã hủy
                    var UpdateAppointmentReply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsRequest, UpdateResponse>(
                        "ExpertServiceUrl",
                        async client => await client.UpdateAppointmentAsync(new GetAppointmentsRequest { AppointmentId = request.AppointmentId, Status = 2 })
                    );
                    // Gửi appointmentId để update status lịch hẹn về trạng thái có thể đặt lịch hẹn
                    var UpdateExpertAvailabilityReply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsRequest, UpdateResponse>(
                        "ExpertServiceUrl",
                        async client => await client.UpdateExpertAvailabilityAsync(new GetAppointmentsRequest { AppointmentId = request.AppointmentId, Status = 0 })
                    );
                    if (UpdateAppointmentReply == null || UpdateExpertAvailabilityReply == null)
                    {
                        return BaseResponse<string>.NotFound("Lịch hẹn không tồn tại.");
                    }
                    return BaseResponse<string>.SuccessReturn("Hủy thanh toán thành công.");
                }
                else if (request.Status == (int)Domain.Enum.PaymentStatus.Paid)
                {
                    await paymentRepository.UpdateStatus(request.OrderCode, Domain.Enum.PaymentStatus.Paid);
                    #region ExpertPaymentService gRPC
                    // Gửi appointmentId qua expert-service để update status của appointment đã đặt lịch hẹn
                    var UpdateAppointmentReply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsRequest, UpdateResponse>(
                        "ExpertServiceUrl",
                        async client => await client.UpdateAppointmentAsync(new GetAppointmentsRequest { AppointmentId = request.AppointmentId, Status = 1 })
                    );
                    // Gửi appointmentId qua expert-service để update status của expertAvailability về đã lịch đã được đặt
                    var UpdateExpertAvailabilityReply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsRequest, UpdateResponse>(
                        "ExpertServiceUrl",
                        async client => await client.UpdateExpertAvailabilityAsync(new GetAppointmentsRequest { AppointmentId = request.AppointmentId, Status = 2 })
                    );
                    // Kiểm tra lịch hẹn có tồn tại không
                    if (UpdateAppointmentReply == null || UpdateExpertAvailabilityReply == null)
                    {
                        return BaseResponse<string>.NotFound("Lịch hẹn không tồn tại.");
                    }
                    // Nếu đã đặt lịch hẹn thành công thì tạo payment QR code

                    // Lấy thông tin thanh toán để tạo payment QR code

                    // Lấy thông tin Appointment của expert-service
                    var GetAppointmentsReply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsRequest, GetAppointmentsResponse>(
                        "ExpertServiceUrl",
                        async client => await client.GetAppointmentsAsync(new GetAppointmentsRequest { AppointmentId = request.AppointmentId })
                    );
                    if (GetAppointmentsReply == null)
                    {
                        return BaseResponse<string>.NotFound("Lịch hẹn không tồn tại.");
                    }
                    // Lấy thông tin thanh toán của user-service để tạo payment QR code
                    // Thông tin thanh toán của user
                    var UserPaymentInfoReply = await grpcHelper.ExecuteGrpcCallAsync<UserService.UserServiceClient, GetUserPaymentInfoRequest, GetPaymentInfoResponse>(
                        "UserServiceUrl",
                        async client => await client.GetUserPaymentInfoAsync(new GetUserPaymentInfoRequest { UserId = GetAppointmentsReply.UserId })
                    );
                    // Thông tin thanh toán của expert
                    var ExpertPaymentInfoReply = await grpcHelper.ExecuteGrpcCallAsync<UserService.UserServiceClient, GetUserPaymentInfoRequest, GetPaymentInfoResponse>(
                        "UserServiceUrl",
                        async client => await client.GetUserPaymentInfoAsync(new GetUserPaymentInfoRequest { UserId = GetAppointmentsReply.ExpertId })
                    );
                    // Kiểm tra thông tin thanh toán của user và expert có tồn tại không
                    if (UserPaymentInfoReply == null || ExpertPaymentInfoReply == null)
                    {
                        return BaseResponse<string>.NotFound("Không tìm thấy thông tin thanh toán.");
                    }
                    // Lấy thông tin phí của nền tảng để tính toán tiền thanh toán cho expert và tiền hoàn lại cho user
                    var platformFee = await platformFeeRepository.GetByPropertyAsync(p => p.PlatformFeeName == "PlatformFee");
                    if (platformFee == null)
                    {
                        return BaseResponse<string>.NotFound("Không tìm thấy thông tin phí.");
                    }
                    // Create QrCode Using vietqr.vn từ thông tin thanh toán của user và expert và thông tin lịch hẹn
                    var userQrCode = CreateQrCode(UserPaymentInfoReply.AccountNumber, UserPaymentInfoReply.BankName, UserPaymentInfoReply.AccountName, GetAppointmentsReply.Amount.ToString(), "Thanh toan lich hen");
                    var expertQrCode = CreateQrCode(ExpertPaymentInfoReply.AccountNumber, ExpertPaymentInfoReply.BankName, ExpertPaymentInfoReply.AccountName, CaculateAmount(GetAppointmentsReply.Amount, platformFee.PlatformFeeValue).ToString(), "Thanh toan lich hen");
                    // Update payment with QrCode
                    var paymentIndb = await paymentRepository.GetByPropertyAsync(p=>p.AppointmentId == request.AppointmentId);
                    if(paymentIndb == null)
                    {
                        return BaseResponse<string>.NotFound("Không tìm thấy thông tin thanh toán.");
                    }
                    paymentIndb.UserPaymentQrCodeLink = userQrCode;
                    paymentIndb.ExpertPaymentQrCodeLink = expertQrCode;
        
                    await paymentRepository.Update(paymentIndb.PaymentId, paymentIndb);
                    // End Update payment with QrCode

                    //End Grpc
                    #endregion
                    return BaseResponse<string>.SuccessReturn("Thanh toán thành công.");
                }
                return BaseResponse<string>.BadRequest("Trạng thái thanh toán không hợp lệ.");
            }
            catch (Exception ex)
            {
                return BaseResponse<string>.InternalServerError(ex.Message);
            }
        }
        // Hàm tạo QrCode sử dụng viqr.vn
        public string CreateQrCode(string accountNumber,string bankName,string accountName, string amount,string description)
        {
            return $"https://api.viqr.net/vietqr/{bankName}/{accountNumber}/{amount}/compact2.jpg?FullName={accountName}&NDck={description}";
        }
        // Hàm tính toán số tiền thanh toán cho expert và số tiền hoàn lại cho user
        // Tiền thanh toán cho expert = amount - (amount * percent)
        // Tiền hoàn lại cho user = amount
        public int CaculateAmount(int amount, int percent)
        {
            // Caculate amount = amount - (amount * percent)
            return amount - (amount * percent / 100);
        }
    }
}