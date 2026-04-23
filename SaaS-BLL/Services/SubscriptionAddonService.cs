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

public class SubscriptionAddonService : ISubscriptionAddonService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public SubscriptionAddonService(IUnitOfWork uow, IMapper mapper)
    {
        this._uow = uow;
        this._mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<SubscriptionAddonDto>>> GetBySubscriptionIdAsync(int subscriptionId, CancellationToken ct = default)
    {
        var addons = await this._uow.SubscriptionAddons.Query()
            .Include(sa => sa.PlanAddon)
            .Where(sa => sa.SubscriptionId == subscriptionId)
            .ToListAsync(ct);

        return ServiceResult<IEnumerable<SubscriptionAddonDto>>.Ok(this._mapper.Map<IEnumerable<SubscriptionAddonDto>>(addons));
    }

    public async Task<ServiceResult<SubscriptionAddonDto>> AddAddonAsync(AddSubscriptionAddonRequest request, CancellationToken ct = default)
    {
        if (request.Quantity < 1)
        {
            return ServiceResult<SubscriptionAddonDto>.Fail("Quantity must be at least 1.");
        }

        var subscription = await this._uow.Subscriptions.GetByIdAsync(request.SubscriptionId, ct);
        if (subscription is null)
        {
            return ServiceResult<SubscriptionAddonDto>.Fail("Subscription not found.");
        }

        if (subscription.State is not(SubscriptionState.Active or SubscriptionState.Trial))
        {
            return ServiceResult<SubscriptionAddonDto>.Fail("Addons can only be added to active or trial subscriptions.");
        }

        var addonExists = await this._uow.PlanAddons.ExistsAsync(request.PlanAddonId, ct);
        if (!addonExists)
        {
            return ServiceResult<SubscriptionAddonDto>.Fail("Plan addon not found.");
        }

        var alreadyAdded = await this._uow.SubscriptionAddons.Query()
            .AnyAsync(
                sa => sa.SubscriptionId == request.SubscriptionId
                         && sa.PlanAddonId == request.PlanAddonId, ct);

        if (alreadyAdded)
        {
            return ServiceResult<SubscriptionAddonDto>.Fail("This addon is already added to the subscription.");
        }

        var subscriptionAddon = new SubscriptionAddon(
            id: 0,
            subscriptionId: request.SubscriptionId,
            planAddonId: request.PlanAddonId,
            quantity: request.Quantity,
            addedDate: DateTime.UtcNow);

        await this._uow.SubscriptionAddons.AddAsync(subscriptionAddon, ct);
        await this._uow.SaveChangesAsync(ct);

        var created = await this._uow.SubscriptionAddons.Query()
            .Include(sa => sa.PlanAddon)
            .FirstAsync(sa => sa.Id == subscriptionAddon.Id, ct);

        return ServiceResult<SubscriptionAddonDto>.Ok(this._mapper.Map<SubscriptionAddonDto>(created));
    }

    public async Task<ServiceResult> RemoveAddonAsync(int id, CancellationToken ct = default)
    {
        var addon = await this._uow.SubscriptionAddons.GetByIdAsync(id, ct);
        if (addon is null)
        {
            return ServiceResult.Fail("Subscription addon not found.");
        }

        this._uow.SubscriptionAddons.Delete(addon);
        await this._uow.SaveChangesAsync(ct);

        return ServiceResult.Ok();
    }
}