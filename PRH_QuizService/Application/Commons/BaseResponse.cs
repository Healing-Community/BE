using Domain.Entities.DASS21;
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
    public DateTime Timestamp { get; set; }
    internal static BaseResponse<T> NotFound(string message = "Không tìm thấy dữ liệu")
    {
        return new BaseResponse<T>
        {
            Id = NewId.NextSequentialGuid().ToString(),
            StatusCode = StatusCodes.Status404NotFound,
            Message = message,
            Success = false,
            Timestamp = DateTime.UtcNow + TimeSpan.FromHours(7) // UTC+7
        };
    }

    internal static BaseResponse<T> SuccessReturn(T classInstance)
    {
        return new BaseResponse<T>
        {
            Id = Ulid.NewUlid().ToString(),
            StatusCode = StatusCodes.Status200OK,
            Message = "Thành công",
            Success = true,
            Data = classInstance,
            Timestamp = DateTime.UtcNow + TimeSpan.FromHours(7) // UTC+7
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
            Timestamp = DateTime.UtcNow + TimeSpan.FromHours(7) // UTC+7
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
            Timestamp = DateTime.UtcNow + TimeSpan.FromHours(7) // UTC+7
        };
    }

    internal static BaseResponse<T> BadRequest(string v)
    {
        return new BaseResponse<T>
        {
            Id = Ulid.NewUlid().ToString(),
            StatusCode = StatusCodes.Status400BadRequest,
            Message = v,
            Success = false,
            Timestamp = DateTime.UtcNow + TimeSpan.FromHours(7) // UTC+7
        };
    }
}