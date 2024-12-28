using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace Application.Interfaces.Services;

public class GrpcHelper(IConfiguration configuration) : IGrpcHelper
{
    public async Task<TResponse?> ExecuteGrpcCallAsync<TClient, TRequest, TResponse>(
        Func<TClient, Task<TResponse>> grpcCall,
        Func<HttpClientHandler>? configureHttpHandler = null)
        where TClient : class
        where TRequest : class
        where TResponse : class
    {
        var serviceUrl = configuration["ExpertServiceUrl"];
        if (string.IsNullOrEmpty(serviceUrl))
        {
            throw new ArgumentException("Service URL cannot be null or empty.", nameof(serviceUrl));
        }

        var httpHandler = configureHttpHandler?.Invoke() ?? new HttpClientHandler
        {
            // Default: allow insecure HTTP/2 for local development
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using var channel = GrpcChannel.ForAddress(serviceUrl, new GrpcChannelOptions
        {
            HttpHandler = httpHandler
        });

        var client = Activator.CreateInstance(typeof(TClient), channel) as TClient;

        if (client == null)
        {
            throw new InvalidOperationException($"Failed to create gRPC client of type {typeof(TClient).FullName}.");
        }

        try
        {
            return await grpcCall(client);
        }
        catch (RpcException ex)
        {
            // Log and rethrow or handle the error
            Console.Error.WriteLine($"gRPC call failed: {ex.Message}");
            throw;
        }
    }
}
