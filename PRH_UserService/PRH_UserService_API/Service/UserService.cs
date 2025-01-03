using Application.Commands_Queries.Queries.GetPaymentInfo.GetPaymentInfoForGrpc;
using Grpc.Core;

namespace PRH_UserService_API.Service;

public class UserService(ISender sender) : UserPaymentService.UserService.UserServiceBase
{
    public override async Task<UserPaymentService.GetPaymentInfoResponse> GetUserPaymentInfo(UserPaymentService.GetUserPaymentInfoRequest request, ServerCallContext context)
    {
        var response = await sender.Send(new GetPaymentInfoGrpcQuery(request.UserId));
        return new UserPaymentService.GetPaymentInfoResponse
        {
            AccountName = response.Data?.BankAccountName,
            AccountNumber = response.Data?.BankAccountNumber,
            BankName = response.Data?.BankName
        };
    }
}
