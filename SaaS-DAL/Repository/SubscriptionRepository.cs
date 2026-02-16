using SaaS_DAL.Data;
using SaaS_DAL.Interfaces;
using SaaS_Domain.Entities;

namespace SaaS_DAL.Repository;

public class SubscriptionRepository : AbstractRepository<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(SaaSDbContext context)
        : base(context)
    {
    }
}