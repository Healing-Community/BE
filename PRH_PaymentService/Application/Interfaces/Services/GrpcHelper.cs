using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace Application.Interfaces.Services;

public class GrpcHelper(IConfiguration configuration) : IGrpcHelper
{
    public async Task<TResponse?> ExecuteGrpcCallAsync<TClient, TRequest, TResponse>(
        string serviceUrlKey,
        Func<TClient, Task<TResponse>> grpcCall,
        Func<HttpClientHandler>? configureHttpHandler = null)
        where TClient : class
        where TRequest : class
        where TResponse : class
    {
        // Retrieve the URL from configuration
        var serviceUrl = configuration[serviceUrlKey];
        if (string.IsNullOrEmpty(serviceUrl))
        {
            throw new ArgumentException($"Service URL for key '{serviceUrlKey}' cannot be null or empty.", nameof(serviceUrlKey));
        }

        // Create or configure the HTTP handler
        var httpHandler = configureHttpHandler?.Invoke() ?? new HttpClientHandler
        {
            // Default: allow insecure HTTP/2 for local development
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        // Create the gRPC channel
        using var channel = GrpcChannel.ForAddress(serviceUrl, new GrpcChannelOptions
        {
            HttpHandler = httpHandler
        });

        // Create the gRPC client
        var client = Activator.CreateInstance(typeof(TClient), channel) as TClient;

        if (client == null)
        {
            throw new InvalidOperationException($"Failed to create gRPC client of type {typeof(TClient).FullName}.");
        }

        try
        {
            // Execute the gRPC call
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
