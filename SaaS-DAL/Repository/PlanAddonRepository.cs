using SaaS_DAL.Data;
using SaaS_DAL.Interfaces;
using SaaS_Domain.Entities;

namespace SaaS_DAL.Repository;

public class PlanAddonRepository : AbstractRepository<PlanAddon>, IPlanAddonRepository
{
    public PlanAddonRepository (SaaSDbContext context) 
        : base(context)
    {
    }
}