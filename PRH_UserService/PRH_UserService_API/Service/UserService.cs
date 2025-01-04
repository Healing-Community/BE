using Application.Commands_Queries.Queries.GetPaymentInfo.GetPaymentInfoForGrpc;
using Application.Commands_Queries.Queries.Users.GetUsersById;
using Grpc.Core;

namespace PRH_UserService_API.Service;

public class UserService(ISender sender) : UserPaymentService.UserService.UserServiceBase
{
    public override async Task<UserPaymentService.GetPaymentInfoResponse> GetUserPaymentInfo(UserPaymentService.GetUserPaymentInfoRequest request, ServerCallContext context)
    {
        var paymentResponse = await sender.Send(new GetPaymentInfoGrpcQuery(request.UserId));
        var userResponse = await sender.Send(new GetUsersByIdQuery(request.UserId));
        return new UserPaymentService.GetPaymentInfoResponse
        {
            UserEmail = userResponse.Data?.Email,
            UserName = userResponse.Data?.UserName,
            AccountName = paymentResponse.Data?.BankAccountName,
            AccountNumber = paymentResponse.Data?.BankAccountNumber,
            BankName = paymentResponse.Data?.BankName
        };
    }
}
