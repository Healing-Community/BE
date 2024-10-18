using System.Net;
using System.Net.Mail;
using Application.Interfaces.Repository;
using Microsoft.Extensions.Configuration;

namespace Persistence.Repositories;

public class EmailRepository(IConfiguration configuration) : IEmailRepository
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        if (string.IsNullOrEmpty(to)) throw new ArgumentException("Email address cannot be null or empty", nameof(to));

        var smtpSettings = configuration.GetSection("SmtpSettings");
        var host = smtpSettings["Host"];
        var port = smtpSettings["Port"];
        var username = smtpSettings["Username"];
        var password = smtpSettings["Password"];
        var fromEmail = smtpSettings["FromEmail"];
        var fromName = smtpSettings["FromName"];

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(username) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(fromName))
            throw new ArgumentException("SMTP settings are not properly configured.");

        var smtpClient = new SmtpClient(host)
        {
            Port = int.Parse(port),
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(to);

        await smtpClient.SendMailAsync(mailMessage);
    }

}