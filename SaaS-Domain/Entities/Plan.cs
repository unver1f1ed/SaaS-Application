using System.ComponentModel.DataAnnotations.Schema;
using SaaS_Domain.Enums;

namespace SaaS_Domain.Entities;

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
    
    public PlanType PlanType { get; set; } 
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public Currency Currency { get; set; }
    public BillingInterval BillingInterval { get; set; }
    public int TrialDays { get; set; }
    
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public virtual ICollection<PlanAddon>  PlanAddons { get; set; } = new List<PlanAddon>();
}