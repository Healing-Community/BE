using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commons;
public class BaseResponseCount<T>
{
    public required string Id { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public bool Success { get; set; }
    public int Total { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public DateTime Timestamp { get; set; }
    

    internal static BaseResponseCount<T> NotFound(string message = "Không tìm thấy dữ liệu")
    {
        return new BaseResponseCount<T>
        {
            Id = Ulid.NewUlid().ToString(),
            StatusCode = StatusCodes.Status404NotFound,
            Message = message,
            Success = false,
            Timestamp = DateTime.UtcNow + TimeSpan.FromHours(7)
        };
    }

    internal static BaseResponseCount<T> SuccessReturn(T classInstance = default, string message = "Thành công", int total = 0)
    {
        return new BaseResponseCount<T>
        {
            Id = Ulid.NewUlid().ToString(),
            StatusCode = StatusCodes.Status200OK,
            Message = message,
            Success = true,
            Data = classInstance,
            Timestamp = DateTime.UtcNow + TimeSpan.FromHours(7),
            Total = total
        };
    }
    internal static BaseResponseCount<T> InternalServerError(string message)
    {
        return new BaseResponseCount<T>
        {
            Id = Ulid.NewUlid().ToString(),
            StatusCode = StatusCodes.Status500InternalServerError,
            Message = message,
            Success = false,
            Timestamp = DateTime.UtcNow + TimeSpan.FromHours(7)
        };
    }

    internal static BaseResponseCount<T> Unauthorized()
    {
        return new BaseResponseCount<T>
        {
            Id = Ulid.NewUlid().ToString(),
            StatusCode = StatusCodes.Status401Unauthorized,
            Message = "Không có quyền truy cập, chưa đăng nhập hoặc phiên làm việc hết hạn",
            Success = false,
            Timestamp = DateTime.UtcNow + TimeSpan.FromHours(7)
        };
    }

    internal static BaseResponseCount<T> BadRequest(string message)
    {
        return new BaseResponseCount<T>
        {
            Id = Ulid.NewUlid().ToString(),
            StatusCode = StatusCodes.Status400BadRequest,
            Message = message,
            Success = false,
            Timestamp = DateTime.UtcNow + TimeSpan.FromHours(7)
        };
    }
}
