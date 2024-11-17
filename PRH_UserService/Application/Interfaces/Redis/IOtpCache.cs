namespace Application.Interfaces.Redis;

public interface IOtpCache
{
    Task<bool> SaveOtpAsync(string key, string otp, TimeSpan expiry);
    Task<string?> GetOtpAsync(string key);

    Task<bool> DeleteOtpAsync(string key);

    // Kiểm tra xem một OTP có tồn tại trong Redis không
    Task<bool> OtpExistsAsync(string key);
}