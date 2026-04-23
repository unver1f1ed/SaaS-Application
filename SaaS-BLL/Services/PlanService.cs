using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SaaS_BLL.Common;
using SaaS_BLL.Interfaces;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;
using SaaS_Domain.Entities;
using SaaS_Domain.Interfaces;

namespace SaaS_BLL.Services;

public class PlanService : IPlanService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public PlanService(IUnitOfWork uow, IMapper mapper)
    {
        this._uow = uow;
        this._mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<PlanDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var plans = await this._uow.Plans.GetAllAsync(ct);
        return ServiceResult<IEnumerable<PlanDto>>.Ok(this._mapper.Map<IEnumerable<PlanDto>>(plans));
    }

    public async Task<ServiceResult<PlanDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var plan = await this._uow.Plans.GetByIdAsync(id, ct);
        return plan is null ? ServiceResult<PlanDto>.Fail("Plan not found.") : ServiceResult<PlanDto>.Ok(this._mapper.Map<PlanDto>(plan));
    }

    public async Task<ServiceResult<PlanDto>> CreateAsync(CreatePlanRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ServiceResult<PlanDto>.Fail("Plan name is required.");
        }

        if (request.BasePrice < 0)
        {
            return ServiceResult<PlanDto>.Fail("Base price cannot be negative.");
        }

        var plan = new Plan(
            id: 0,
            planType: request.PlanType,
            name: request.Name.Trim(),
            description: request.Description?.Trim(),
            basePrice: request.BasePrice,
            currency: request.Currency,
            billingInterval: request.BillingInterval,
            trialDays: request.TrialDays);

        await this._uow.Plans.AddAsync(plan, ct);
        await this._uow.SaveChangesAsync(ct);

        return ServiceResult<PlanDto>.Ok(this._mapper.Map<PlanDto>(plan));
    }

    public async Task<ServiceResult<PlanDto>> UpdateAsync(int id, UpdatePlanRequest request, CancellationToken ct = default)
    {
        var plan = await this._uow.Plans.GetByIdAsync(id, ct);
        if (plan is null)
        {
            return ServiceResult<PlanDto>.Fail("Plan not found.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ServiceResult<PlanDto>.Fail("Plan name is required.");
        }

        if (request.BasePrice < 0)
        {
            return ServiceResult<PlanDto>.Fail("Base price cannot be negative.");
        }

        plan.PlanType = request.PlanType;
        plan.Name = request.Name.Trim();
        plan.Description = request.Description?.Trim();
        plan.BasePrice = request.BasePrice;
        plan.Currency = request.Currency;
        plan.BillingInterval = request.BillingInterval;
        plan.TrialDays = request.TrialDays;

        this._uow.Plans.Update(plan);
        await this._uow.SaveChangesAsync(ct);

        return ServiceResult<PlanDto>.Ok(this._mapper.Map<PlanDto>(plan));
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken ct = default)
    {
        var plan = await this._uow.Plans.GetByIdAsync(id, ct);
        if (plan is null)
        {
            return ServiceResult.Fail("Plan not found.");
        }

        // Check if plan has any subscriptions
        var hasSubscriptions = await this._uow.Subscriptions.Query()
            .AnyAsync(s => s.PlanId == id, ct);

        if (hasSubscriptions)
        {
            return ServiceResult.Fail("Cannot delete plan with existing subscriptions. Please delete all subscriptions first.");
        }

        // Get all plan addons
        var planAddons = await this._uow.PlanAddons.Query()
            .Where(pa => pa.PlanId == id)
            .ToListAsync(ct);

        // Delete each addon (this will check for SubscriptionAddons)
        foreach (var addon in planAddons)
        {
            // Check if addon has subscription addons
            var hasSubscriptionAddons = await this._uow.SubscriptionAddons.Query()
                .AnyAsync(sa => sa.PlanAddonId == addon.Id, ct);

            if (hasSubscriptionAddons)
            {
                return ServiceResult.Fail($"Cannot delete plan because addon '{addon.AddonName}' is being used by subscriptions.");
            }

            this._uow.PlanAddons.Delete(addon);
        }

        // Now delete the plan
        this._uow.Plans.Delete(plan);
        await this._uow.SaveChangesAsync(ct);

        return ServiceResult.Ok();
    }
}