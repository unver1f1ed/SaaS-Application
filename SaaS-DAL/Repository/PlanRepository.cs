using SaaS_DAL.Data;
using SaaS_Domain.Entities;
using SaaS_Domain.Interfaces;

namespace SaaS_DAL.Repository;

public class PlanRepository : AbstractRepository<Plan>, IPlanRepository
{
    public PlanRepository(SaaSDbContext context)
        : base(context)
    {
    }
}