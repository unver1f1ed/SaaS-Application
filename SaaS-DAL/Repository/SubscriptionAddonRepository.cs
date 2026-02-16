using SaaS_DAL.Data;
using SaaS_DAL.Interfaces;
using SaaS_Domain.Entities;

namespace SaaS_DAL.Repository;

public class SubscriptionAddonRepository : AbstractRepository<SubscriptionAddon>, ISubscriptionAddonRepository
{
    public SubscriptionAddonRepository(SaaSDbContext context)
        : base(context)
    {
    }
}