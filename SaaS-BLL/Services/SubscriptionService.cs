using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SaaS_BLL.Common;
using SaaS_BLL.Interfaces;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;
using SaaS_Domain.Entities;
using SaaS_Domain.Enums;
using SaaS_Domain.Interfaces;

namespace SaaS_BLL.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public SubscriptionService(IUnitOfWork uow, IMapper mapper)
    {
        this._uow = uow;
        this._mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<SubscriptionDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var subscriptions = await this._uow.Subscriptions.Query()
            .Include(s => s.Plan)
            .Include(s => s.User)
            .ToListAsync(ct);

        return ServiceResult<IEnumerable<SubscriptionDto>>.Ok(this._mapper.Map<IEnumerable<SubscriptionDto>>(subscriptions));
    }

    public async Task<ServiceResult<SubscriptionDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var subscription = await this._uow.Subscriptions.Query()
            .Include(s => s.Plan)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        return subscription is null ? ServiceResult<SubscriptionDto>.Fail("Subscription not found.") : ServiceResult<SubscriptionDto>.Ok(this._mapper.Map<SubscriptionDto>(subscription));
    }

    public async Task<ServiceResult<IEnumerable<SubscriptionDto>>> GetByUserIdAsync(int userId, CancellationToken ct = default)
    {
        var subscriptions = await this._uow.Subscriptions.Query()
            .Include(s => s.Plan)
            .Where(s => s.UserId == userId)
            .ToListAsync(ct);

        return ServiceResult<IEnumerable<SubscriptionDto>>.Ok(this._mapper.Map<IEnumerable<SubscriptionDto>>(subscriptions));
    }

    public async Task<ServiceResult<SubscriptionDto>> SubscribeAsync(CreateSubscriptionRequest request, CancellationToken ct = default)
    {
        var userExists = await this._uow.Users.ExistsAsync(request.UserId, ct);
        if (!userExists)
        {
            return ServiceResult<SubscriptionDto>.Fail("User not found.");
        }

        var plan = await this._uow.Plans.GetByIdAsync(request.PlanId, ct);
        if (plan is null)
        {
            return ServiceResult<SubscriptionDto>.Fail("Plan not found.");
        }

        var now = DateTime.UtcNow;

        // Get all active subscriptions for this user
        var activeSubscriptions = await this._uow.Subscriptions.Query()
            .Where(s => s.UserId == request.UserId
                        && (s.State == SubscriptionState.Active || s.State == SubscriptionState.Trial))
            .ToListAsync(ct);

        // Automatically cancel all existing active subscriptions
        if (activeSubscriptions.Any())
        {
            foreach (var oldSubscription in activeSubscriptions)
            {
                oldSubscription.State = SubscriptionState.Cancelled;
                oldSubscription.CancellationDate = now;
                oldSubscription.CancellationReason = "Automatically cancelled due to new subscription";
                oldSubscription.AutoRenew = false;
                this._uow.Subscriptions.Update(oldSubscription);
            }
        }

        var isTrial = plan.TrialDays > 0;

        var subscription = new Subscription(
            id: 0,
            userId: request.UserId,
            planId: request.PlanId,
            state: isTrial ? SubscriptionState.Trial : SubscriptionState.Active,
            startDate: now,
            endDate: null,
            trialEndDate: isTrial ? now.AddDays(plan.TrialDays) : null,
            autoRenew: request.AutoRenew,
            cancellationDate: null,
            cancellationReason: null);

        await this._uow.Subscriptions.AddAsync(subscription, ct);
        await this._uow.SaveChangesAsync(ct);

        // Create payment record for all paid subscriptions
        if (plan.BasePrice > 0)
        {
            // For trial subscriptions, create a pending payment
            // For non-trial subscriptions, create a completed payment
            var payment = new Payment(
                id: 0,
                subscriptionId: subscription.Id,
                amount: plan.BasePrice,
                currency: plan.Currency,
                paymentDate: now,
                paymentMethod: PaymentMethod.CreditCard,
                status: isTrial ? PaymentStatus.Pending : PaymentStatus.Completed);

            await this._uow.Payments.AddAsync(payment, ct);
            await this._uow.SaveChangesAsync(ct);
        }

        return await this.GetByIdAsync(subscription.Id, ct);
    }

    public async Task<ServiceResult<SubscriptionDto>> UpdateAsync(int id, UpdateSubscriptionRequest request, CancellationToken ct = default)
    {
        var subscription = await this._uow.Subscriptions.GetByIdAsync(id, ct);
        if (subscription is null)
        {
            return ServiceResult<SubscriptionDto>.Fail("Subscription not found.");
        }

        subscription.AutoRenew = request.AutoRenew;

        this._uow.Subscriptions.Update(subscription);
        await this._uow.SaveChangesAsync(ct);

        return await this.GetByIdAsync(id, ct);
    }

    public async Task<ServiceResult<SubscriptionDto>> CancelAsync(int id, CancelSubscriptionRequest request, CancellationToken ct = default)
    {
        var subscription = await this._uow.Subscriptions.GetByIdAsync(id, ct);
        if (subscription is null)
        {
            return ServiceResult<SubscriptionDto>.Fail("Subscription not found.");
        }

        switch (subscription.State)
        {
            case SubscriptionState.Cancelled:
                return ServiceResult<SubscriptionDto>.Fail("Subscription is already cancelled.");
            case SubscriptionState.Expired:
                return ServiceResult<SubscriptionDto>.Fail("Cannot cancel an expired subscription.");
        }

        subscription.State = SubscriptionState.Cancelled;
        subscription.CancellationDate = DateTime.UtcNow;
        subscription.CancellationReason = request.CancellationReason?.Trim();
        subscription.AutoRenew = false;

        this._uow.Subscriptions.Update(subscription);
        await this._uow.SaveChangesAsync(ct);

        return await this.GetByIdAsync(id, ct);
    }

    public async Task<ServiceResult<SubscriptionDto>> RenewAsync(int id, CancellationToken ct = default)
    {
        var subscription = await this._uow.Subscriptions.Query()
            .Include(s => s.Plan)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        if (subscription is null)
        {
            return ServiceResult<SubscriptionDto>.Fail("Subscription not found.");
        }

        if (subscription.State is not(SubscriptionState.Active or SubscriptionState.Expired))
        {
            return ServiceResult<SubscriptionDto>.Fail("Only active or expired subscriptions can be renewed.");
        }

        subscription.State = SubscriptionState.Active;
        subscription.StartDate = DateTime.UtcNow;
        subscription.EndDate = null;
        subscription.CancellationDate = null;
        subscription.CancellationReason = null;

        this._uow.Subscriptions.Update(subscription);
        await this._uow.SaveChangesAsync(ct);

        return await this.GetByIdAsync(id, ct);
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken ct = default)
    {
        var subscription = await this._uow.Subscriptions.GetByIdAsync(id, ct);
        if (subscription is null)
        {
            return ServiceResult.Fail("Subscription not found.");
        }

        this._uow.Subscriptions.Delete(subscription);
        await this._uow.SaveChangesAsync(ct);

        return ServiceResult.Ok();
    }
}