namespace SaaS_DAL.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Repositories (injected via DI)
    IUserRepository Users { get; }
    IUserRoleRepository UserRoles { get; }
    IPlanRepository Plans { get; }
    ISubscriptionRepository Subscriptions { get; }
    IPaymentRepository Payments { get; }
    IPlanAddonRepository PlanAddons { get; }
    ISubscriptionAddonRepository SubscriptionAddons { get; }
    
    // Save changes
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    // Transaction support
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}