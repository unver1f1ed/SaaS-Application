using SaaS_Domain.Enums;

namespace SaaS_BLL.Models;

public record PlanDto(
    int Id,
    PlanType PlanType,
    string Name,
    string? Description,
    decimal BasePrice,
    Currency Currency,
    BillingInterval BillingInterval,
    int TrialDays);