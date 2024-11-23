using Grpc.Net.Client;
using ExpertService.gRPC;

namespace PRH_PaymentService_API.Services
{
    public class ExpertServiceGrpcClient
    {
        private readonly ExpertService.gRPC.ExpertService.ExpertServiceClient _client;

        public ExpertServiceGrpcClient(string expertServiceUrl)
        {
            var channel = GrpcChannel.ForAddress(expertServiceUrl);
            _client = new ExpertService.gRPC.ExpertService.ExpertServiceClient(channel);
        }

        public async Task<PaymentSuccessResponse> PaymentSuccessAsync(PaymentSuccessRequest request)
        {
            return await _client.PaymentSuccessAsync(request);
        }
    }
}
