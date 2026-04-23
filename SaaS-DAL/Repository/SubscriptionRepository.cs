using SaaS_DAL.Data;
using SaaS_Domain.Entities;
using SaaS_Domain.Interfaces;

namespace SaaS_DAL.Repository;

public class SubscriptionRepository : AbstractRepository<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(SaaSDbContext context)
        : base(context)
    {
    }
}