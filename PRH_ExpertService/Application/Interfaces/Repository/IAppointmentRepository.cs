using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IAppointmentRepository : IReadRepository<Appointment>, ICreateRepository<Appointment>, IUpdateRepository<Appointment>, IDeleteRepository
    {
        Task<IEnumerable<Appointment>> GetByExpertProfileIdAsync(string expertProfileId);
        Task<IEnumerable<Appointment>> GetOverlappingAppointmentsAsync(string expertProfileId, DateOnly appointmentDate, TimeOnly startTime, TimeOnly endTime);
        Task<IEnumerable<Appointment>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Appointment>> GetAppointmentsToCompleteAsync(DateTime currentTime);
    }
}
