using SaaS_Domain.Entities;

namespace SaaS_DAL.Interfaces;

/// <summary>
/// Defines a generic repository contract for performing CRUD operations
/// and querying entities of type <typeparamref name="TEntity"/>.
/// Provides asynchronous methods for data retrieval and persistence,
/// as well as queryable access for advanced scenarios.
/// </summary>
/// <typeparam name="TEntity">
/// The type of entity managed by the repository. Must inherit from <see cref="BaseEntity"/>.
/// </typeparam>
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    
    IQueryable<TEntity> Query();
    
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    void Update(TEntity entity);
    
    void Delete(TEntity entity);
    
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}