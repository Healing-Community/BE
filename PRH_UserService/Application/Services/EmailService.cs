
using Application.Interfaces.Services;
using Application.Interfaces.Repository;

namespace Application.Services
{
    public class EmailService(IEmailRepository emailRepository) : IEmailService
    {
        public async Task SendOtpEmailAsync(string toEmail, string otp)
        {
            string emailContent = $@"
                <html>
                <body style=""margin: 0; padding: 0; font-family: 'Verdana', sans-serif; background-color: #f0f4f8;"">
                    <div style=""max-width: 650px; margin: 0 auto; background-color: #fff; padding: 30px; border-radius: 10px; box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);"">
                        <div style=""text-align: center;"">
                            <img src=""https://i.postimg.cc/zXN0D5kY/logo.png"" alt=""Healing Image"" style=""max-width: 100%; height: auto; border-radius: 8px;"">
                        </div>
                        <h2 style=""color: #4caf50; text-align: center; margin-top: 20px;"">Mã OTP của bạn là:</h2>
                        <h1 style=""color: #4caf50; text-align: center; margin-top: 20px;"">{otp}</h1>
                        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">Đây là mã OTP của bạn. Vui lòng không chia sẻ mã này với bất kỳ ai khác.</p>
                        <p style=""font-size: 14px; color: #666; text-align: center; margin-top: 30px;"">Nếu bạn không yêu cầu mã OTP, vui lòng bỏ qua email này.</p>
                        <p style=""text-align: center; color: #999; font-size: 13px;"">&copy; 2024 Healing Community. Tất cả các quyền được bảo lưu.</p>
                    </div>
                </body>
                </html>
                ";
            await emailRepository.SendEmailAsync(toEmail, "Mã OTP của bạn", emailContent);
        }

        public async Task SendVerificationEmailAsync(string toEmail, string verificationLink)
        {
            // Nội dung email
            string emailContent = $@"
                <html>
                <body style=""margin: 0; padding: 0; font-family: 'Verdana', sans-serif; background-color: #f0f4f8;"">
                    <div style=""max-width: 650px; margin: 0 auto; background-color: #fff; padding: 30px; border-radius: 10px; box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);"">
                        <div style=""text-align: center;"">
                            <img src=""https://i.postimg.cc/zXN0D5kY/logo.png"" alt=""Healing Image"" style=""max-width: 100%; height: auto; border-radius: 8px;"">
                        </div>
                        <h2 style=""color: #4caf50; text-align: center; margin-top: 20px;"">Chào mừng bạn đến với Cộng đồng Chữa lành!</h2>
                        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">Cảm ơn bạn đã tin tưởng và đăng ký với chúng tôi. Hãy xác thực tài khoản của bạn bằng cách nhấn vào liên kết dưới đây để bắt đầu hành trình chữa lành của bạn.</p>
                        <div style=""text-align: center; margin-top: 20px;"">
                            <a href=""{verificationLink}"" style=""background-color: #4caf50; color: #ffffff; padding: 12px 30px; border-radius: 5px; text-decoration: none; font-size: 18px;"">Xác thực Email</a>
                        </div>
                        <p style=""font-size: 14px; color: #666; text-align: center; margin-top: 30px;"">Nếu bạn không yêu cầu đăng ký, vui lòng bỏ qua email này.</p>
                        <p style=""text-align: center; color: #999; font-size: 13px;"">&copy; 2024 Healing Community. Tất cả các quyền được bảo lưu.</p>
                    </div>
                </body>
                </html>
                ";

            // Gửi email xác minh
            await emailRepository.SendEmailAsync(toEmail, "Xác thực email của bạn", emailContent);
        }
    }
}
