using Microsoft.EntityFrameworkCore;
using SaaS_DAL.Data;
using SaaS_Domain.Entities;
using SaaS_Domain.Interfaces;

namespace SaaS_DAL.Repository;

/// <summary>
/// Base repository implementation using Entity Framework Core.
/// Provides common CRUD operations for entities.
/// </summary>
/// <typeparam name="TEntity">
/// The entity type managed by the repository. Must inherit from <see cref="BaseEntity"/>.
/// </typeparam>
public abstract class AbstractRepository<TEntity> : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly SaaSDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected AbstractRepository(SaaSDbContext context)
    {
        this.Context = context;
        this.DbSet = context.Set<TEntity>();
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await this.DbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await this.DbSet.FindAsync([id], cancellationToken);
    }

    public virtual IQueryable<TEntity> Query()
    {
        return this.DbSet.AsQueryable();
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await this.DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual void Update(TEntity entity)
    {
        this.DbSet.Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        this.DbSet.Remove(entity);
    }

    public virtual async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await this.DbSet
            .AsNoTracking()
            .AnyAsync(e => e.Id == id, cancellationToken);
    }
}