using SaaS_Domain.Enums;

namespace SaaS_BLL.Models.Requests;

public class AddSubscriptionAddonRequest
{
    public int SubscriptionId { get; init; }

    public int PlanAddonId { get; init; }

    public int Quantity { get; init; } = 1;
}

public class RecordPaymentRequest
{
    public int SubscriptionId { get; set; }

    public decimal Amount { get; set; }

    public Currency Currency { get; set; }

    public PaymentMethod PaymentMethod { get; set; }
}