using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace Application.Services
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {
        private static string GetEmailTemplate(string title, string bodyContent, string footer = "&copy; 2024 Healing Community. Tất cả các quyền được bảo lưu.")
        {
            return $@"
            <html>
            <body style=""margin: 0; padding: 0; font-family: 'Verdana', sans-serif; background-color: #e0f7fa;"">
                <div style=""max-width: 650px; margin: 0 auto; background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);"">
                    <div style=""text-align: center;"">
                        <img src=""https://i.postimg.cc/zXN0D5kY/logo.png"" alt=""Healing Community Logo"" style=""max-width: 150px; height: auto; margin-bottom: 20px;"">
                    </div>
                    <h2 style=""color: #00796b; text-align: center; margin-top: 20px;"">{title}</h2>
                    <div style=""font-size: 17px; line-height: 1.8; color: #444; margin-top: 20px;"">
                        {bodyContent}
                    </div>
                    <p style=""text-align: center; color: #999; font-size: 13px; margin-top: 30px;"">{footer}</p>
                </div>
            </body>
            </html>
            ";
        }

        public async Task SendAppointmentConfirmationEmailAsync(string toEmail, string appointmentTime, string meetingLink)
        {
            var bodyContent = $@"
                <p>Xin chào,</p>
                <p>Bạn đã đặt lịch hẹn vào lúc <strong>{appointmentTime}</strong>.</p>
                <p>Liên kết cuộc họp của bạn: <a href=""{meetingLink}"">{meetingLink}</a></p>
                <p>Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của Healing Community.</p>";
            var emailContent = GetEmailTemplate("Xác nhận cuộc hẹn", bodyContent);
            await SendEmailAsync(toEmail, "Xác nhận cuộc hẹn", emailContent);
        }

        public async Task SendAppointmentNotificationToExpertAsync(string toEmail, string appointmentTime, string meetingLink)
        {
            var bodyContent = $@"
                <p>Xin chào,</p>
                <p>Bạn có một lịch hẹn mới vào lúc <strong>{appointmentTime}</strong>.</p>
                <p>Liên kết cuộc họp: <a href=""{meetingLink}"">{meetingLink}</a></p>
                <p>Vui lòng chuẩn bị và tham gia đúng giờ.</p>
                <p>Cảm ơn bạn đã đồng hành cùng Healing Community.</p>";
            var emailContent = GetEmailTemplate("Thông báo lịch hẹn mới", bodyContent);
            await SendEmailAsync(toEmail, "Thông báo lịch hẹn mới", emailContent);
        }

        public async Task SendAppointmentCancellationEmailAsync(string toEmail, string appointmentTime)
        {
            var bodyContent = $@"
                <p>Xin chào,</p>
                <p>Chúng tôi rất tiếc thông báo rằng lịch hẹn vào lúc <strong>{appointmentTime}</strong> của bạn đã bị hủy.</p>
                <p>Nếu bạn cần hỗ trợ hoặc có bất kỳ thắc mắc nào, vui lòng liên hệ với chúng tôi.</p>
                <p>Cảm ơn bạn đã quan tâm đến Healing Community.</p>";
            var emailContent = GetEmailTemplate("Thông báo hủy lịch hẹn", bodyContent);
            await SendEmailAsync(toEmail, "Thông báo hủy lịch hẹn", emailContent);
        }

        private async Task SendEmailAsync(string to, string subject, string body)
        {
            if (string.IsNullOrEmpty(to))
                throw new ArgumentException("Địa chỉ email không được để trống.", nameof(to));

            var smtpSettings = configuration.GetSection("SmtpSettings");
            var host = smtpSettings["Host"];
            var port = smtpSettings["Port"];
            var username = smtpSettings["Username"];
            var password = smtpSettings["Password"];
            var fromEmail = smtpSettings["FromEmail"];
            var fromName = smtpSettings["FromName"];

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(fromName))
                throw new ArgumentException("Cấu hình SMTP không đầy đủ.");

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
