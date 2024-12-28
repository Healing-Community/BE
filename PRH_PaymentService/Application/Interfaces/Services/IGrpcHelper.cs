using System;

namespace Application.Interfaces.Services;

public interface IGrpcHelper
{
    Task<TResponse?> ExecuteGrpcCallAsync<TClient, TRequest, TResponse>(
        Func<TClient, Task<TResponse>> grpcCall,
        Func<HttpClientHandler>? configureHttpHandler = null)
        where TClient : class
        where TRequest : class
        where TResponse : class;
}
