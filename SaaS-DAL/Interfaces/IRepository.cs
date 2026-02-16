using SaaS_Domain.Entities;

namespace SaaS_DAL.Interfaces;

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