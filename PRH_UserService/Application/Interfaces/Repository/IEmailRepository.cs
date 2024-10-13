namespace Application.Interfaces.Repository;

public interface IEmailRepository
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendPasswordResetEmailAsync(string to, string resetLink);
}