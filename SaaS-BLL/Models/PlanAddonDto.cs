using SaaS_Domain.Enums;

namespace SaaS_BLL.Models;

public record PlanAddonDto(
    int Id,
    int PlanId,
    string AddonName,
    string? Description,
    decimal UnitPrice,
    Currency Currency,
    BillingType BillingType);