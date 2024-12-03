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
            Id = Ulid.NewUlid().ToString(),
            StatusCode = StatusCodes.Status404NotFound,
            Message = message,
            Success = false,
            Timestamp = DateTime.UtcNow + TimeSpan.FromHours(7)
        };
    }

    internal static BaseResponse<T> SuccessReturn(T classInstance = default, string message = "Thành công")
    {
        return new BaseResponse<T>
        {
            Id = Ulid.NewUlid().ToString(),
            StatusCode = StatusCodes.Status200OK,
            Message = message,
            Success = true,
            Data = classInstance,
            Timestamp = DateTime.UtcNow + TimeSpan.FromHours(7)
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
            Timestamp = DateTime.UtcNow + TimeSpan.FromHours(7)
        };
    }

    internal static BaseResponse<T> Unauthorized()
    {
        return new BaseResponse<T>
        {
            Id = Ulid.NewUlid().ToString(),
            StatusCode = StatusCodes.Status401Unauthorized,
            Message = "Không có quyền truy cập, chưa đăng nhập hoặc phiên làm việc hết hạn",
            Success = false,
            Timestamp = DateTime.UtcNow + TimeSpan.FromHours(7)
        };
    }

    internal static BaseResponse<string> BadRequest(string message)
    {
        return new BaseResponse<string>
        {
            Id = Ulid.NewUlid().ToString(),
            StatusCode = StatusCodes.Status400BadRequest,
            Message = message,
            Success = false,
            Timestamp = DateTime.UtcNow + TimeSpan.FromHours(7)
        };
    }
}
