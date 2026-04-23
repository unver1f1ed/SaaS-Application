using Microsoft.EntityFrameworkCore;
using SaaS_DAL.Data.InitDataFactory;

namespace SaaS_DAL.Data;

/// <summary>
/// Factory for creating <see cref="SaaSDbContext"/> instances with
/// a SQLite database connection and optional initial data seeding.
/// </summary>
public class SaaSDbFactory
{
    private readonly AbstractDataFactory _factory;
    private readonly string? _connectionString;

    public SaaSDbFactory(AbstractDataFactory factory, string? connectionString = null)
    {
        this._factory = factory;
        this._connectionString = connectionString;
    }

    public SaaSDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SaaSDbContext>()
            .UseSqlServer(this._connectionString)
            .Options;

        var context = new SaaSDbContext(options, this._factory);

        context.Database.Migrate();

        return context;
    }
}