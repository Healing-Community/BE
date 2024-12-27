using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enum;
using ExpertService.gRPC;
using Grpc.Core;
using Newtonsoft.Json;

namespace PRH_PaymentService_API.Services
{
    public class PaymentStatusPollingService(
        IServiceScopeFactory serviceScopeFactory,
        IMapper mapper,
        IConfiguration configuration,
        ILogger<PaymentStatusPollingService> logger) : BackgroundService
    {
        private const int MaxDegreeOfParallelism = 5;
        private static readonly TimeSpan PollingInterval = TimeSpan.FromMinutes(1);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var startTime = DateTime.UtcNow;

                try
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();

                    var pendingPayments = await paymentRepository.GetPendingPaymentsAsync();

                    await Parallel.ForEachAsync(pendingPayments, new ParallelOptions
                    {
                        MaxDegreeOfParallelism = MaxDegreeOfParallelism,
                        CancellationToken = stoppingToken
                    }, async (payment, token) =>
                    {
                        await ProcessPaymentAsync(payment, token);
                    });
                }
                catch (Exception ex)
                {
                    HandlePollingError(ex);
                }

                var elapsedTime = DateTime.UtcNow - startTime;
                var delayTime = PollingInterval - elapsedTime;

                if (delayTime > TimeSpan.Zero)
                {
                    await Task.Delay(delayTime, stoppingToken);
                }
            }
        }

        private async Task ProcessPaymentAsync(Payment payment, CancellationToken cancellationToken)
        {
            try
            {
                using var scope = serviceScopeFactory.CreateScope();
                var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
                var payOSService = scope.ServiceProvider.GetRequiredService<IPayOSService>();

                if ((PaymentStatus)payment.Status == PaymentStatus.Pending)
                {
                    var payOSResponse = await payOSService.GetPaymentStatus(payment.OrderCode);

                    if (payOSResponse != null)
                    {
                        var paymentStatus = mapper.Map<PaymentStatus>(payOSResponse.Status);

                        if (paymentStatus != (PaymentStatus)payment.Status)
                        {
                            await paymentRepository.UpdateStatus(payment.OrderCode, paymentStatus);

                            var paymentSuccessRequest = new PaymentSuccessRequest
                            {
                                AppointmentId = payment.AppointmentId,
                                PaymentId = payment.PaymentId,
                                IsSuccess = paymentStatus == PaymentStatus.Paid
                            };

                            try
                            {
                                var expertServiceResponse = await CreateExpertServiceClient()
                                    .PaymentSuccessAsync(paymentSuccessRequest);

                                if (!expertServiceResponse.Success)
                                {
                                    logger.LogWarning(
                                        "Không thể thông báo cho Expert Service cho AppointmentId: {AppointmentId}. Message: {Message}",
                                        payment.AppointmentId, expertServiceResponse.Message);
                                }
                            }
                            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
                            {
                                logger.LogError(ex,
                                    "Lỗi khi kết nối với Expert Service cho AppointmentId: {AppointmentId}",
                                    payment.AppointmentId);
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex,
                                    "Lỗi khi thông báo cho Expert Service cho AppointmentId: {AppointmentId}",
                                    payment.AppointmentId);
                            }
                        }
                    }
                }
            }
            catch (JsonReaderException ex)
            {
                logger.LogError(ex, "Lỗi khi phân tích cú pháp JSON cho OrderCode {OrderCode}", payment.OrderCode);
            }
            catch (Exception ex)
            {
                HandleIndividualError(payment.OrderCode, ex);
            }
        }

        private ExpertServiceGrpcClient CreateExpertServiceClient()
        {
            var expertServiceUrl = configuration["ExpertServiceUrl"]
                ?? throw new InvalidOperationException("ExpertServiceUrl is not configured.");

            return new ExpertServiceGrpcClient(expertServiceUrl);
        }

        private void HandleIndividualError(long orderCode, Exception exception)
        {
            logger.LogError(exception, "Lỗi khi cập nhật thanh toán với OrderCode {OrderCode}", orderCode);
        }

        private void HandlePollingError(Exception exception)
        {
            logger.LogError(exception, "Lỗi xảy ra khi polling trạng thái thanh toán");
        }
    }
}
