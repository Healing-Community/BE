using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace Persistence.Repositories
{
    public class PaymentRepository(PaymentDbContext context) : IPaymentRepository
    {
        public async Task Create(Payment entity)
        {
            await context.Payments.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var payment = await context.Payments.FindAsync(id);
            if (payment != null)
            {
                context.Payments.Remove(payment);
                await context.SaveChangesAsync();
            }
        }

        public async Task<Payment?> GetByIdAsync(string id)
        {
            return await context.Payments.FindAsync(id);
        }

        public async Task<Payment?> GetByOrderCodeAsync(string orderCode)
        {
            return await context.Payments.FirstOrDefaultAsync(p => p.AppointmentId == orderCode);
        }

        public async Task<Payment?> GetByPropertyAsync(Expression<Func<Payment, bool>> predicate)
        {
            return await context.Payments.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Payment>> GetsAsync()
        {
            return await context.Payments.ToListAsync();
        }

        public async Task Update(string id, Payment entity)
        {
            var existingPayment = await context.Payments.FindAsync(id);
            if (existingPayment != null)
            {
                context.Entry(existingPayment).CurrentValues.SetValues(entity);
                await context.SaveChangesAsync();
            }
        }
    }
}
