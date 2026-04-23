using SaaS_DAL.Data;
using SaaS_Domain.Entities;
using SaaS_Domain.Interfaces;

namespace SaaS_DAL.Repository;

public class PlanAddonRepository : AbstractRepository<PlanAddon>, IPlanAddonRepository
{
    public PlanAddonRepository(SaaSDbContext context)
        : base(context)
    {
    }
}