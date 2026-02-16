using Microsoft.EntityFrameworkCore.Storage;
using SaaS_DAL.Data;
using SaaS_DAL.Interfaces;

namespace SaaS_DAL.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly SaaSDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repositories injected via constructor
    public IUserRepository Users { get; }
    public IUserRoleRepository UserRoles { get; }
    public IPlanRepository Plans { get; }
    public ISubscriptionRepository Subscriptions { get; }
    public IPaymentRepository Payments { get; }
    public IPlanAddonRepository PlanAddons { get; }
    public ISubscriptionAddonRepository SubscriptionAddons { get; }

    public UnitOfWork(
        SaaSDbContext context,
        IUserRepository users,
        IUserRoleRepository userRoles,
        IPlanRepository plans,
        ISubscriptionRepository subscriptions,
        IPaymentRepository payments,
        IPlanAddonRepository planAddons,
        ISubscriptionAddonRepository subscriptionAddons)
    {
        _context = context;
        Users = users;
        UserRoles = userRoles;
        Plans = plans;
        Subscriptions = subscriptions;
        Payments = payments;
        PlanAddons = planAddons;
        SubscriptionAddons = subscriptionAddons;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}