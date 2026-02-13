using System.ComponentModel.DataAnnotations.Schema;
using SaaS_Domain.Enums;

namespace SaaS_Domain.Entities;

[Table("subscriptions")]
public class Subscription : BaseEntity
{
    public Subscription() 
        : base()
    {
    }

    public Subscription(int id, int userId, int planId, SubscriptionState state, 
        DateTime startDate, DateTime? endDate, DateTime? trialEndDate, bool autoRenew, 
        DateTime? cancellationDate, string? cancellationReason)
        : base(id)
    {
        this.UserId = userId;
        this.PlanId = planId;
        this.State = state;
        this.StartDate = startDate;
        this.EndDate = endDate;
        this.TrialEndDate = trialEndDate;
        this.AutoRenew = autoRenew;
        this.CancellationDate = cancellationDate;
        this.CancellationReason = cancellationReason;
    }
    
    [Column("user_id")] public int UserId { get; set; }
    
    [Column("plan_id")] public int PlanId { get; set; }
    
    [Column("state")] public SubscriptionState State { get; set; }
    
    [Column("start_date")] public DateTime StartDate { get; set; }
    
    [Column("end_date")] public DateTime? EndDate { get; set; }
    
    [Column("trial_end_date")] public DateTime? TrialEndDate { get; set; }
    
    [Column("auto_renew")] public bool AutoRenew { get; set; }
    
    [Column("cancellation_date")] public DateTime? CancellationDate { get; set; }
    
    [Column("cancellation_reason")] public string? CancellationReason { get; set; }

    public User User { get; set; } = null!;

    public Plan Plan { get; set; } = null!;
    
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<SubscriptionAddon> SubscriptionAddons { get; set; } = new List<SubscriptionAddon>();
}