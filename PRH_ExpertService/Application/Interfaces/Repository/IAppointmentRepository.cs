using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IAppointmentRepository : IReadRepository<Appointment>, ICreateRepository<Appointment>, IUpdateRepository<Appointment>, IDeleteRepository
    {
    }
}
