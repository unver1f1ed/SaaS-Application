using SaaS_DAL.Data;
using SaaS_Domain.Entities;
using SaaS_Domain.Interfaces;

namespace SaaS_DAL.Repository;

public class SubscriptionAddonRepository : AbstractRepository<SubscriptionAddon>, ISubscriptionAddonRepository
{
    public SubscriptionAddonRepository(SaaSDbContext context)
        : base(context)
    {
    }
}