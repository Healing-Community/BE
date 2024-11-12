using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Enum;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class PaymentStatusPollingService(
        IServiceScopeFactory serviceScopeFactory,
        IMapper mapper,
        ILogger<PaymentStatusPollingService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Starting Payment Status Polling Service...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
                    var payOSService = scope.ServiceProvider.GetRequiredService<IPayOSService>();

                    var pendingPayments = await paymentRepository.GetsAsync();

                    foreach (var payment in pendingPayments)
                    {
                        if ((PaymentStatus)payment.Status == PaymentStatus.Pending)
                        {
                            var payOSResponse = await payOSService.GetPaymentStatus(payment.OrderCode);

                            if (payOSResponse != null)
                            {
                                var paymentStatus = mapper.Map<PaymentStatus>(payOSResponse.Status);
                                if (paymentStatus != (PaymentStatus)payment.Status)
                                {
                                    await paymentRepository.UpdateStatus(payment.OrderCode, paymentStatus);
                                    logger.LogInformation($"Updated order {payment.OrderCode} to status {paymentStatus}");
                                }
                            }
                        }
                    }

                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while polling payment status.");
                }
            }
        }
    }
}
