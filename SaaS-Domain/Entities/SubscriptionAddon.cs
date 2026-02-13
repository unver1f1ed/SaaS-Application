using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS_Domain.Entities;

[Table("subscription_addons")]
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
    
    [Column("subscription_id")] public int SubscriptionId { get; set; }
    
    [Column("plan_addon_id")] public int PlanAddonId { get; set; }

    [Column("quantity")] public int Quantity { get; set; }
    
    [Column("added_date")] public DateTime AddedDate { get; set; }

    public Subscription Subscription { get; set; } = null!;

    public PlanAddon PlanAddon { get; set; } = null!;
}