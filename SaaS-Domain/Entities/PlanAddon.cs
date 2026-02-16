using System.ComponentModel.DataAnnotations.Schema;
using SaaS_Domain.Enums;

namespace SaaS_Domain.Entities;

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
    
    public int PlanId { get; set; }
    public string AddonName { get; set; } = null!;
    public string? Description { get; set; }
    public decimal UnitPrice { get; set; }
    public Currency Currency { get; set; }
    public BillingType BillingType { get; set; }

    public Plan Plan { get; set; } = null!;
    public virtual ICollection<SubscriptionAddon> SubscriptionAddons { get; set; } = new List<SubscriptionAddon>();
}