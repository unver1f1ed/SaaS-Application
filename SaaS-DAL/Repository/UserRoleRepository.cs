using SaaS_DAL.Data;
using SaaS_Domain.Entities;
using SaaS_Domain.Interfaces;

namespace SaaS_DAL.Repository;

public class UserRoleRepository : AbstractRepository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(SaaSDbContext context)
        : base(context)
    {
    }
}