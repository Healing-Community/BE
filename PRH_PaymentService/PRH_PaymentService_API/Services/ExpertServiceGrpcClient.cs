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

        public async Task<GetAppointmentDetailsResponse> GetAppointmentDetailsAsync(string appointmentId)
        {
            var request = new GetAppointmentDetailsRequest
            {
                AppointmentId = appointmentId
            };
            return await _client.GetAppointmentDetailsAsync(request);
        }
    }
}
