using System.ComponentModel.DataAnnotations.Schema;
using SaaS_Domain.Enums;

namespace SaaS_Domain.Entities;

[Table("payments")]
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
    
    [Column("subscription_id")] public int SubscriptionId { get; set; }
    
    [Column("amount")] public decimal Amount { get; set; }
    
    [Column("currency")] public Currency Currency { get; set; }
    
    [Column("payment_date")] public DateTime PaymentDate { get; set; }
    
    [Column("payment_method")] public PaymentMethod PaymentMethod { get; set; }
    
    [Column("status")] public PaymentStatus Status { get; set; }

    public Subscription Subscription { get; set; } = null!;

}