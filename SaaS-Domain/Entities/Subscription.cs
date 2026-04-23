using SaaS_Domain.Enums;

namespace SaaS_Domain.Entities;

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

    public int UserId { get; init; }

    public int PlanId { get; init; }

    public SubscriptionState State { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? TrialEndDate { get; init; }

    public bool AutoRenew { get; set; }

    public DateTime? CancellationDate { get; set; }

    public string? CancellationReason { get; set; }

    public User User { get; init; } = null!;

    public Plan Plan { get; init; } = null!;

    public virtual ICollection<Payment> Payments { get; init; } = new List<Payment>();

    public virtual ICollection<SubscriptionAddon> SubscriptionAddons { get; init; } = new List<SubscriptionAddon>();
}