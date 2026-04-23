using SaaS_Domain.Enums;

namespace SaaS_BLL.Models.Requests;

public class CreatePlanRequest
{
    public PlanType PlanType { get; init; }

    public string Name { get; init; } = null!;

    public string? Description { get; init; }

    public decimal BasePrice { get; init; }

    public Currency Currency { get; init; }

    public BillingInterval BillingInterval { get; init; }

    public int TrialDays { get; init; }
}

public class UpdatePlanRequest
{
    public PlanType PlanType { get; init; }

    public string Name { get; init; } = null!;

    public string? Description { get; init; }

    public decimal BasePrice { get; init; }

    public Currency Currency { get; init; }

    public BillingInterval BillingInterval { get; init; }

    public int TrialDays { get; init; }
}