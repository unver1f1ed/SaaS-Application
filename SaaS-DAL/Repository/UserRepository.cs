using SaaS_DAL.Data;
using SaaS_Domain.Entities;
using SaaS_Domain.Interfaces;

namespace SaaS_DAL.Repository;

public class UserRepository : AbstractRepository<User>, IUserRepository
{
    public UserRepository(SaaSDbContext context)
        : base(context)
    {
    }
}