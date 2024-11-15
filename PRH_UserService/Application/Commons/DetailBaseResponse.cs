using MassTransit;
using Microsoft.AspNetCore.Http;

namespace Application.Commons;

public class DetailBaseResponse<T>
{
    public required string Id { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public bool Success { get; set; }
    public T? Data { get; set; }
    public List<ErrorDetail>? Errors { get; set; }
    public DateTime Timestamp { get; set; }

    internal static DetailBaseResponse<T> SuccessReturn(string Message,T classInstance)
    {
        return new DetailBaseResponse<T>
        {
            Id = NewId.NextSequentialGuid().ToString(),
            StatusCode = StatusCodes.Status200OK,
            Message = Message,
            Success = true,
            Data = classInstance,
            Timestamp = DateTime.UtcNow
        };
    }
}