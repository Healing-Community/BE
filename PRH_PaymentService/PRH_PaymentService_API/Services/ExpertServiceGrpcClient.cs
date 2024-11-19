using Grpc.Net.Client;
using ExpertService.gRPC;

namespace PRH_PaymentService_API.Services
{
    public class ExpertServiceGrpcClient
    {
        private readonly ExpertService.gRPC.ExpertService.ExpertServiceClient _client;

        public ExpertServiceGrpcClient(string expertServiceUrl)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var channel = GrpcChannel.ForAddress(expertServiceUrl, new GrpcChannelOptions
            {
                HttpHandler = httpHandler
            });
            _client = new ExpertService.gRPC.ExpertService.ExpertServiceClient(channel);
        }

        public async Task<PaymentSuccessResponse> PaymentSuccessAsync(PaymentSuccessRequest request)
        {
            return await _client.PaymentSuccessAsync(request);
        }
    }
}
