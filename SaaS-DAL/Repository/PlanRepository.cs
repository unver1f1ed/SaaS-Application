using SaaS_DAL.Data;
using SaaS_DAL.Interfaces;
using SaaS_Domain.Entities;

namespace SaaS_DAL.Repository;

public class PlanRepository : AbstractRepository<Plan>, IPlanRepository
{
    public PlanRepository(SaaSDbContext context)
        : base(context)
    {
    }
}