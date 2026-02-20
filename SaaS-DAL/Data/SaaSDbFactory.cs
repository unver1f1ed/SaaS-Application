using Microsoft.Data.Sqlite;
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
    private readonly string _dbPath;

    public SaaSDbFactory(AbstractDataFactory factory, string? dbPath = null)
    {
        this._factory = factory;
        this._dbPath = dbPath ?? "DB/saas.db";
    }

    public SaaSDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SaaSDbContext>()
            .UseSqlite(CreateConnectionString())
            .Options;
        
        var context = new SaaSDbContext(options, this._factory);
        
        context.Database.Migrate();
        
        return context;
    }

    private string CreateConnectionString()
    {
        return new SqliteConnectionStringBuilder
        {
            DataSource = _dbPath,
            Mode = SqliteOpenMode.ReadWriteCreate,
        }.ConnectionString;
    }
}