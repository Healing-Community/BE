using MassTransit;
using Microsoft.AspNetCore.Http;
using NUlid;

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
    internal static BaseResponse<T> NotFound(string message = "Không tìm thấy dữ liệu")
    {
        return new BaseResponse<T>
        {
            Id = NewId.NextSequentialGuid().ToString(),
            StatusCode = StatusCodes.Status404NotFound,
            Message = message,
            Success = false,
            Timestamp = DateTimeOffset.UtcNow
        };
    }

    internal static BaseResponse<T> SuccessReturn(T classInstance)
    {
        return new BaseResponse<T>
        {
            Id = Ulid.NewUlid().ToString(),
            StatusCode = StatusCodes.Status200OK,
            Message = "thành công",
            Success = true,
            Data = classInstance,
            Timestamp = DateTimeOffset.UtcNow
        };
    }
    internal static BaseResponse<T> InternalServerError(string message)
    {
        return new BaseResponse<T>
        {
            Id = Ulid.NewUlid().ToString(),
            StatusCode = StatusCodes.Status500InternalServerError,
            Message = message,
            Success = false,
            Timestamp = DateTimeOffset.UtcNow
        };
    }

    internal static BaseResponse<string> Unauthorized()
    {
        return new BaseResponse<string>
        {
            Id = Ulid.NewUlid().ToString(),
            StatusCode = StatusCodes.Status401Unauthorized,
            Message = "Không có quyền truy cập, chưa đăng nhập hoặc phiên làm việc hết hạn",
            Success = false,
            Timestamp = DateTimeOffset.UtcNow
        };
    }
}