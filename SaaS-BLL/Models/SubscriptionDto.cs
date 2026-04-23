using SaaS_Domain.Enums;

namespace SaaS_BLL.Models;

public record SubscriptionDto(
    int Id,
    int UserId,
    int PlanId,
    string PlanName,
    SubscriptionState State,
    DateTime StartDate,
    DateTime? EndDate,
    DateTime? TrialEndDate,
    bool AutoRenew,
    DateTime? CancellationDate,
    string? CancellationReason);