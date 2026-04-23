using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SaaS_BLL.Common;
using SaaS_BLL.Interfaces;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;
using SaaS_Domain.Entities;
using SaaS_Domain.Interfaces;

namespace SaaS_BLL.Services;

public class PlanAddonService : IPlanAddonService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public PlanAddonService(IUnitOfWork uow, IMapper mapper)
    {
        this._uow = uow;
        this._mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<PlanAddonDto>>> GetByPlanIdAsync(int planId, CancellationToken ct = default)
    {
        var addons = await this._uow.PlanAddons.Query()
            .Where(pa => pa.PlanId == planId)
            .ToListAsync(ct);

        return ServiceResult<IEnumerable<PlanAddonDto>>.Ok(this._mapper.Map<IEnumerable<PlanAddonDto>>(addons));
    }

    public async Task<ServiceResult<PlanAddonDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var addon = await this._uow.PlanAddons.GetByIdAsync(id, ct);
        return addon is null ? ServiceResult<PlanAddonDto>.Fail("Plan addon not found.") : ServiceResult<PlanAddonDto>.Ok(this._mapper.Map<PlanAddonDto>(addon));
    }

    public async Task<ServiceResult<PlanAddonDto>> CreateAsync(CreatePlanAddonRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.AddonName))
        {
            return ServiceResult<PlanAddonDto>.Fail("Addon name is required.");
        }

        if (request.UnitPrice < 0)
        {
            return ServiceResult<PlanAddonDto>.Fail("Unit price cannot be negative.");
        }

        var planExists = await this._uow.Plans.ExistsAsync(request.PlanId, ct);
        if (!planExists)
        {
            return ServiceResult<PlanAddonDto>.Fail("Plan not found.");
        }

        var addon = new PlanAddon(
            id: 0,
            planId: request.PlanId,
            addonName: request.AddonName.Trim(),
            description: request.Description?.Trim(),
            unitPrice: request.UnitPrice,
            currency: request.Currency,
            billingType: request.BillingType);

        await this._uow.PlanAddons.AddAsync(addon, ct);
        await this._uow.SaveChangesAsync(ct);

        return ServiceResult<PlanAddonDto>.Ok(this._mapper.Map<PlanAddonDto>(addon));
    }

    public async Task<ServiceResult<PlanAddonDto>> UpdateAsync(int id, UpdatePlanAddonRequest request, CancellationToken ct = default)
    {
        var addon = await this._uow.PlanAddons.GetByIdAsync(id, ct);
        if (addon is null)
        {
            return ServiceResult<PlanAddonDto>.Fail("Plan addon not found.");
        }

        if (string.IsNullOrWhiteSpace(request.AddonName))
        {
            return ServiceResult<PlanAddonDto>.Fail("Addon name is required.");
        }

        if (request.UnitPrice < 0)
        {
            return ServiceResult<PlanAddonDto>.Fail("Unit price cannot be negative.");
        }

        addon.AddonName = request.AddonName.Trim();
        addon.Description = request.Description?.Trim();
        addon.UnitPrice = request.UnitPrice;
        addon.Currency = request.Currency;
        addon.BillingType = request.BillingType;

        this._uow.PlanAddons.Update(addon);
        await this._uow.SaveChangesAsync(ct);

        return ServiceResult<PlanAddonDto>.Ok(this._mapper.Map<PlanAddonDto>(addon));
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken ct = default)
    {
        var addon = await this._uow.PlanAddons.GetByIdAsync(id, ct);
        if (addon is null)
        {
            return ServiceResult.Fail("Plan addon not found.");
        }

        // Check if addon is being used by any subscriptions
        var isUsed = await this._uow.SubscriptionAddons.Query()
            .AnyAsync(sa => sa.PlanAddonId == id, ct);

        if (isUsed)
        {
            return ServiceResult.Fail("Cannot delete addon that is being used by subscriptions.");
        }

        this._uow.PlanAddons.Delete(addon);
        await this._uow.SaveChangesAsync(ct);

        return ServiceResult.Ok();
    }
}