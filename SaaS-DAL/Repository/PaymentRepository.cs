using SaaS_DAL.Data;
using SaaS_DAL.Interfaces;
using SaaS_Domain.Entities;

namespace SaaS_DAL.Repository;

public class PaymentRepository : AbstractRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(SaaSDbContext context) 
        : base(context)
    {
    }
    
}