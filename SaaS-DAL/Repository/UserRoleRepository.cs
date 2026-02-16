using SaaS_DAL.Data;
using SaaS_DAL.Interfaces;
using SaaS_Domain.Entities;

namespace SaaS_DAL.Repository;

public class UserRoleRepository : AbstractRepository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(SaaSDbContext context)
        : base(context)
    {
    }
}