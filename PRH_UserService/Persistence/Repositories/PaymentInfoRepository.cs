using System;
using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;

namespace Persistence.Repositories;

public class PaymentInfoRepository : GenericRepository<PaymentInfo>, IPaymentInfoRepository
{
    public PaymentInfoRepository(UserServiceDbContext context) : base(context)
    {
    }
}
