namespace SaaS_Domain.Entities;

public class SubscriptionAddon : BaseEntity
{
    public SubscriptionAddon()
        : base()
    {
    }

    public SubscriptionAddon(int id, int subscriptionId, int planAddonId,
        int quantity, DateTime addedDate)
        : base(id)
    {
        this.SubscriptionId = subscriptionId;
        this.PlanAddonId = planAddonId;
        this.Quantity = quantity;
        this.AddedDate = addedDate;
    }

    public int SubscriptionId { get; init; }

    public int PlanAddonId { get; init; }

    public int Quantity { get; init; }

    public DateTime AddedDate { get; init; }

    public Subscription Subscription { get; init; } = null!;

    public PlanAddon PlanAddon { get; init; } = null!;
}