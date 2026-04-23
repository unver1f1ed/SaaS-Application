using AutoMapper;
using SaaS_BLL.Models;
using SaaS_Domain.Entities;

namespace SaaS_BLL.Mappings;

public class SaaSMappingProfile : Profile
{
    public SaaSMappingProfile()
    {
        this.CreateMap<User, UserDto>()
            .ConstructUsing(src => new UserDto(
                src.Id,
                src.FirstName,
                src.LastName,
                src.Email,
                src.UserRoleId,
                src.UserRole.RoleName));

        this.CreateMap<UserRole, UserRoleDto>()
            .ConstructUsing(src => new UserRoleDto(
                src.Id,
                src.RoleName,
                src.Description));

        this.CreateMap<Plan, PlanDto>()
            .ConstructUsing(src => new PlanDto(
                src.Id,
                src.PlanType,
                src.Name,
                src.Description,
                src.BasePrice,
                src.Currency,
                src.BillingInterval,
                src.TrialDays));

        this.CreateMap<PlanAddon, PlanAddonDto>()
            .ConstructUsing(src => new PlanAddonDto(
                src.Id,
                src.PlanId,
                src.AddonName,
                src.Description,
                src.UnitPrice,
                src.Currency,
                src.BillingType));

        this.CreateMap<Subscription, SubscriptionDto>()
            .ConstructUsing(src => new SubscriptionDto(
                src.Id,
                src.UserId,
                src.PlanId,
                src.Plan.Name,
                src.State,
                src.StartDate,
                src.EndDate,
                src.TrialEndDate,
                src.AutoRenew,
                src.CancellationDate,
                src.CancellationReason));

        this.CreateMap<SubscriptionAddon, SubscriptionAddonDto>()
            .ConstructUsing(src => new SubscriptionAddonDto(
                src.Id,
                src.SubscriptionId,
                src.PlanAddonId,
                src.PlanAddon.AddonName,
                src.Quantity,
                src.AddedDate));

        this.CreateMap<Payment, PaymentDto>()
            .ConstructUsing(src => new PaymentDto(
                src.Id,
                src.SubscriptionId,
                src.Amount,
                src.Currency,
                src.PaymentDate,
                src.PaymentMethod,
                src.Status));
    }
}