using Quartz;
using Application.Interfaces.Repository;
using Microsoft.Extensions.Logging;

namespace Application.Jobs
{
    [DisallowConcurrentExecution]
    public class AppointmentStatusJob(
        IAppointmentRepository appointmentRepository,
        ILogger<AppointmentStatusJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("AppointmentStatusJob is running.");

            try
            {
                var currentTime = DateTime.UtcNow.AddHours(7);

                // Lấy danh sách các cuộc hẹn (đã lọc cục bộ)
                var appointmentsToUpdate = await appointmentRepository.GetAppointmentsToCompleteAsync(currentTime);

                foreach (var appointment in appointmentsToUpdate)
                {
                    appointment.Status = 3; // Completed
                    appointment.UpdatedAt = DateTime.UtcNow.AddHours(7);
                    await appointmentRepository.Update(appointment.AppointmentId, appointment);
                }

                logger.LogInformation($"Updated {appointmentsToUpdate.Count()} appointment schedule to 'Completed'.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred when updating appointment status in AppointmentStatusJob.");
            }
        }
    }
}
