namespace Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendAppointmentConfirmationEmailAsync(string toEmail, string appointmentTime, string meetingLink);
        Task SendAppointmentNotificationToExpertAsync(string toEmail, string appointmentTime, string meetingLink);
        Task SendAppointmentCancellationEmailAsync(string toEmail, string appointmentTime);
    }
}
