using Microsoft.EntityFrameworkCore.Storage;
using SaaS_Domain.Interfaces;

namespace SaaS_DAL.Data;

/// <summary>
/// Implements the Unit of Work pattern for managing repositories,
/// transactions, and committing changes to the database as a single unit.
/// </summary>
public class UnitOfWork(
    SaaSDbContext context,
    IUserRepository users,
    IUserRoleRepository userRoles,
    IPlanRepository plans,
    ISubscriptionRepository subscriptions,
    IPaymentRepository payments,
    IPlanAddonRepository planAddons,
    ISubscriptionAddonRepository subscriptionAddons)
    : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    // Repositories injected via constructor
    public IUserRepository Users { get; } = users;

    public IUserRoleRepository UserRoles { get; } = userRoles;

    public IPlanRepository Plans { get; } = plans;

    public ISubscriptionRepository Subscriptions { get; } = subscriptions;

    public IPaymentRepository Payments { get; } = payments;

    public IPlanAddonRepository PlanAddons { get; } = planAddons;

    public ISubscriptionAddonRepository SubscriptionAddons { get; } = subscriptionAddons;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        this._transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (this._transaction != null)
        {
            await this._transaction.CommitAsync(cancellationToken);
            await this._transaction.DisposeAsync();
            this._transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (this._transaction != null)
        {
            await this._transaction.RollbackAsync(cancellationToken);
            await this._transaction.DisposeAsync();
            this._transaction = null;
        }
    }

    public void Dispose()
    {
        this._transaction?.Dispose();
        context.Dispose();
    }
}