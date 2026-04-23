using SaaS_Domain.Enums;

namespace SaaS_BLL.Models;

public record PaymentDto(
    int Id,
    int SubscriptionId,
    decimal Amount,
    Currency Currency,
    DateTime PaymentDate,
    PaymentMethod PaymentMethod,
    PaymentStatus Status);