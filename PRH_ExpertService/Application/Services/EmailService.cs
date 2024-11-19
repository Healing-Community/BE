using System.Net;
using System.Net.Mail;
using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {
        public async Task SendAppointmentConfirmationEmailAsync(string toEmail, string expertName, string appointmentTime, string meetingLink)
        {
            var emailContent = $@"
                <html>
                <body style=""font-family: 'Verdana', sans-serif;"">
                    <div style=""max-width: 650px; margin: 0 auto; padding: 30px;"">
                        <h2>Thông báo xác nhận cuộc hẹn</h2>
                        <p>Chào bạn,</p>
                        <p>Bạn đã đặt lịch hẹn với chuyên gia <strong>{expertName}</strong> vào lúc <strong>{appointmentTime}</strong>.</p>
                        <p>Link họp của bạn: <a href=""{meetingLink}"">{meetingLink}</a></p>
                        <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.</p>
                        <p>Trân trọng,<br/>Healing Community</p>
                    </div>
                </body>
                </html>
                ";

            await SendEmailAsync(toEmail, "Xác nhận cuộc hẹn", emailContent);
        }

        private async Task SendEmailAsync(string to, string subject, string body)
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
}
