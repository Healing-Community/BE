namespace Application.Commons;

public class RegisterUserResponse<T>
{
    public required string Id { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public bool Success { get; set; }
    public T? Data { get; set; }
    public List<ErrorDetail>? Errors { get; set; }
    public DateTime Timestamp { get; set; }
}