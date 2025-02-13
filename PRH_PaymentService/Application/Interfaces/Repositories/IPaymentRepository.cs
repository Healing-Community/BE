﻿using System.Linq.Expressions;
using Application.Interfaces.GenericRepository;
using Domain.Entities;
using Domain.Enum;

namespace Application.Interfaces.Repositories
{
    public interface IPaymentRepository : IReadRepository<Payment>, ICreateRepository<Payment>, IUpdateRepository<Payment>, IDeleteRepository
    {
        Task<Payment?> GetByOrderCodeAsync(long orderCode);
        Task UpdateStatus(long orderCode, PaymentStatus status);
        Task<IEnumerable<Payment>> GetPendingPaymentsAsync();
        Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(string userId);
        Task<IEnumerable<Payment>> GetsByPropertyAsync(Expression<Func<Payment, bool>> predicate);
    }
}
