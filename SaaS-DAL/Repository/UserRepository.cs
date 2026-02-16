using SaaS_DAL.Data;
using SaaS_DAL.Interfaces;
using SaaS_Domain.Entities;

namespace SaaS_DAL.Repository;

public class UserRepository : AbstractRepository<User>, IUserRepository
{
    public UserRepository(SaaSDbContext context)
        : base(context)
    {
    }
}