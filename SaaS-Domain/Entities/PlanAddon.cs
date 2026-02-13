using System.ComponentModel.DataAnnotations.Schema;
using SaaS_Domain.Enums;

namespace SaaS_Domain.Entities;

[Table("plan_addons")]
public class PlanAddon : BaseEntity
{
    public PlanAddon()
        : base()
    {
    }

    public PlanAddon(int id, int planId, string addonName,string? description, 
        decimal unitPrice, Currency currency, BillingType billingType)
        : base(id)
    {
        this.PlanId =  planId;
        this.AddonName =  addonName;
        this.Description = description;
        this.UnitPrice = unitPrice;
        this.Currency = currency;
        this.BillingType = billingType;
    }
    
    [Column("plan_id")] public int PlanId { get; set; }

    [Column("addon_name")] public string AddonName { get; set; } = null!;
    
    [Column("description")] public string? Description { get; set; }
    
    [Column("unit_price")] public decimal UnitPrice { get; set; }
    
    [Column("currency")] public Currency Currency { get; set; }
    
    [Column("billing_type")] public BillingType BillingType { get; set; }

    public Plan Plan { get; set; } = null!;
    
    public virtual ICollection<SubscriptionAddon> SubscriptionAddons { get; set; } = new List<SubscriptionAddon>();
}