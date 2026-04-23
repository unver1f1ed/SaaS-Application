using SaaS_Domain.Enums;

namespace SaaS_BLL.Models.Requests;

public class CreatePlanAddonRequest
{
    public int PlanId { get; init; }

    public string AddonName { get; init; } = null!;

    public string? Description { get; init; }

    public decimal UnitPrice { get; init; }

    public Currency Currency { get; init; }

    public BillingType BillingType { get; init; }
}

public class UpdatePlanAddonRequest
{
    public string AddonName { get; init; } = null!;

    public string? Description { get; init; }

    public decimal UnitPrice { get; init; }

    public Currency Currency { get; init; }

    public BillingType BillingType { get; init; }
}