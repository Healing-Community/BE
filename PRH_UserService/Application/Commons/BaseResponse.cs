using MassTransit;
using Microsoft.AspNetCore.Http;

namespace Application.Commons;

public class BaseResponse<T>
{
    public required string Id { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public bool Success { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public DateTimeOffset Timestamp { get; set; }

    internal static BaseResponse<T> NotFound()
    {
        return new BaseResponse<T>
        {
            Id = NewId.NextSequentialGuid().ToString(),
            StatusCode = StatusCodes.Status404NotFound,
            Message = "không tìm thấy dữ liệu",
            Success = false,
            Timestamp = DateTimeOffset.UtcNow
        };
    }

    internal static BaseResponse<T> SuccessReturn(T classInstance)
    {
        return new BaseResponse<T>
        {
            Id = NewId.NextSequentialGuid().ToString(),
            StatusCode = StatusCodes.Status200OK,
            Message = "thành công",
            Success = true,
            Data = classInstance,
            Timestamp = DateTimeOffset.UtcNow
        };
    }
}