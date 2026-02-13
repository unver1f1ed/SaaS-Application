using System.ComponentModel.DataAnnotations.Schema;
using SaaS_Domain.Enums;

namespace SaaS_Domain.Entities;

[Table("plans")]
public class Plan : BaseEntity
{
    public Plan()
        : base()
    {
    }

    public Plan(int id, PlanType planType,string name, string? description,
        decimal basePrice, Currency currency, BillingInterval billingInterval, int trialDays)
        : base(id)
    {
        this.PlanType =  planType;
        this.Name = name;
        this.Description =  description;
        this.BasePrice = basePrice;
        this.Currency = currency;
        this.BillingInterval = billingInterval;
        this.TrialDays = trialDays;
    }
    
    [Column("plan_type")] public PlanType PlanType { get; set; }

    [Column("name")] public string Name { get; set; } = null!;
    
    [Column("description")] public string? Description { get; set; }
    
    [Column("base_price")] public decimal BasePrice { get; set; }
    
    [Column("currency")] public Currency Currency { get; set; }
    
    [Column("billing_interval")] public BillingInterval BillingInterval { get; set; }
    
    [Column("trial_days")] public int TrialDays { get; set; }
    
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    
    public virtual ICollection<PlanAddon>  PlanAddons { get; set; } = new List<PlanAddon>();
}