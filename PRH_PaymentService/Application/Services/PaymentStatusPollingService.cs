using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Enum;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.Services
{
    public class PaymentStatusPollingService(
        IServiceScopeFactory serviceScopeFactory,
        IMapper mapper) : BackgroundService
    {
        private const int MaxDegreeOfParallelism = 5;
        private static readonly TimeSpan PollingInterval = TimeSpan.FromMinutes(5);

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
                        try
                        {
                            using var innerScope = serviceScopeFactory.CreateScope();
                            var innerPaymentRepository = innerScope.ServiceProvider.GetRequiredService<IPaymentRepository>();
                            var payOSService = innerScope.ServiceProvider.GetRequiredService<IPayOSService>();

                            if ((PaymentStatus)payment.Status == PaymentStatus.Pending)
                            {
                                var payOSResponse = await payOSService.GetPaymentStatus(payment.OrderCode);

                                if (payOSResponse != null)
                                {
                                    var paymentStatus = mapper.Map<PaymentStatus>(payOSResponse.Status);

                                    if (paymentStatus != (PaymentStatus)payment.Status)
                                    {
                                        await innerPaymentRepository.UpdateStatus(payment.OrderCode, paymentStatus);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log or handle individual payment errors as needed
                            HandleIndividualError(payment.OrderCode, ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    // Log or handle overall polling errors as needed
                    HandlePollingError(ex);
                }

                // Calculate the remaining time until the next polling interval
                var elapsedTime = DateTime.UtcNow - startTime;
                var delayTime = PollingInterval - elapsedTime;

                if (delayTime > TimeSpan.Zero)
                {
                    await Task.Delay(delayTime, stoppingToken);
                }
            }
        }

        private void HandleIndividualError(long orderCode, Exception exception)
        {
            // Handle individual payment error (e.g., log or rethrow)
            throw new ApplicationException($"Error updating payment with OrderCode {orderCode}.", exception);
        }

        private void HandlePollingError(Exception exception)
        {
            // Handle polling-wide errors (e.g., log or rethrow)
            throw new ApplicationException("Error occurred while polling payment status.", exception);
        }
    }
}
