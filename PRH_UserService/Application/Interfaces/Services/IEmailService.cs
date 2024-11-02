namespace Application.Interfaces.Services;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string toEmail, string verificationLink);
    Task SendOtpEmailAsync(string toEmail, string otp);
}