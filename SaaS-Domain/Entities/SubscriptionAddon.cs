using System.ComponentModel.DataAnnotations.Schema;

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
        this.PlanAddonId =  planAddonId;
        this.Quantity = quantity;
        this.AddedDate = addedDate;
    }
    
    public int SubscriptionId { get; set; }
    public int PlanAddonId { get; set; }
    public int Quantity { get; set; }
    public DateTime AddedDate { get; set; }

    public Subscription Subscription { get; set; } = null!;
    public PlanAddon PlanAddon { get; set; } = null!;
}