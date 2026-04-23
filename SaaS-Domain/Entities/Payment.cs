using SaaS_Domain.Enums;

namespace SaaS_Domain.Entities;

public class Payment : BaseEntity
{
    public Payment()
        : base()
    {
    }

    public Payment(int id, int subscriptionId, decimal amount, Currency currency,
        DateTime paymentDate, PaymentMethod paymentMethod, PaymentStatus status)
        : base(id)
    {
        this.SubscriptionId = subscriptionId;
        this.Amount = amount;
        this.Currency = currency;
        this.PaymentDate = paymentDate;
        this.PaymentMethod = paymentMethod;
        this.Status = status;
    }

    public int SubscriptionId { get; init; }

    public decimal Amount { get; init; }

    public Currency Currency { get; init; }

    public DateTime PaymentDate { get; init; }

    public PaymentMethod PaymentMethod { get; init; }

    public PaymentStatus Status { get; set; }

    public Subscription Subscription { get; init; } = null!;
}