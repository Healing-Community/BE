﻿using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class AppointmentRepository(ExpertDbContext context) : IAppointmentRepository
    {

        public async Task Create(Appointment entity)
        {
            await context.Appointments.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var appointment = await context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                context.Appointments.Remove(appointment);
                await context.SaveChangesAsync();
            }
        }

        public async Task<Appointment?> GetByIdAsync(string id)
        {
            return await context.Appointments.FindAsync(id);
        }

        public async Task<Appointment?> GetByPropertyAsync(Expression<Func<Appointment, bool>> predicate)
        {
            return await context.Appointments.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Appointment>> GetsAsync()
        {
            return await context.Appointments.ToListAsync();
        }

        public async Task Update(string id, Appointment entity)
        {
            var existingAppointment = await context.Appointments.FindAsync(id);
            if (existingAppointment != null)
            {
                context.Entry(existingAppointment).CurrentValues.SetValues(entity);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Appointment>> GetByExpertProfileIdAsync(string expertProfileId)
        {
            return await context.Appointments
                .Where(a => a.ExpertProfileId == expertProfileId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetOverlappingAppointmentsAsync(string expertProfileId, DateOnly appointmentDate, TimeOnly startTime, TimeOnly endTime)
        {
            return await context.Appointments
                .Where(a => a.ExpertProfileId == expertProfileId &&
                            a.AppointmentDate == appointmentDate &&
                            ((startTime >= a.StartTime && startTime < a.EndTime) ||
                             (endTime > a.StartTime && endTime <= a.EndTime) ||
                             (startTime <= a.StartTime && endTime >= a.EndTime)))
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByUserIdAsync(string userId)
        {
            return await context.Appointments
                    .Include(a => a.ExpertProfile)
                    .Where(a => a.UserId == userId)
                    .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsToCompleteAsync(DateTime currentTime)
        {
            // 1) Lấy hết những cuộc hẹn có Status == 1
            var all = await context.Appointments
                .Where(a => a.Status == 1)
                .ToListAsync();

            // 2) Lọc cục bộ 
            // So sánh AppointmentDate.ToDateTime(EndTime) <= currentTime
            var filtered = all
                .Where(a => a.AppointmentDate.ToDateTime(a.EndTime) <= currentTime);

            return filtered;
        }
    }
}
