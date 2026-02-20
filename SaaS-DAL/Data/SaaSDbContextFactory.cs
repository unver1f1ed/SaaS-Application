using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SaaS_DAL.Data.InitDataFactory;

namespace SaaS_DAL.Data;

/// <summary>
/// Design-time factory for creating <see cref="SaaSDbContext"/> instances.
/// Used by tools like EF Core migrations.
/// </summary>
public class SaaSDbContextFactory : IDesignTimeDbContextFactory<SaaSDbContext>
{
    public SaaSDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SaaSDbContext>();
        optionsBuilder.UseSqlite("Data Source=DB/saas.db");

        var factory = new ReleaseDataFactory();
        
        return new SaaSDbContext(optionsBuilder.Options, factory);
    }
}