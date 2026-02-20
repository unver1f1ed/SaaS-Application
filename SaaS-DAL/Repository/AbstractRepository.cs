using Microsoft.EntityFrameworkCore;
using SaaS_DAL.Data;
using SaaS_DAL.Interfaces;
using SaaS_Domain.Entities;

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
        return await DbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public virtual IQueryable<TEntity> Query()
    {
        return DbSet.AsQueryable();
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public virtual async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .AnyAsync(e => e.Id == id, cancellationToken);
    }
}