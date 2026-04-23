namespace SaaS_BLL.Models.Requests;

public class CreateSubscriptionRequest
{
    public int UserId { get; init; }

    public int PlanId { get; init; }

    public bool AutoRenew { get; init; }
}

public class UpdateSubscriptionRequest
{
    public bool AutoRenew { get; init; }
}

public class CancelSubscriptionRequest
{
    public string? CancellationReason { get; init; }
}