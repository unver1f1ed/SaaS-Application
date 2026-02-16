using System.ComponentModel.DataAnnotations.Schema;
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
    
    public int SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; }

    public Subscription Subscription { get; set; } = null!;

}