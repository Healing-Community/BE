namespace Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendAppointmentConfirmationEmailAsync(string toEmail, string expertName, string appointmentTime, string meetingLink);
    }
}
