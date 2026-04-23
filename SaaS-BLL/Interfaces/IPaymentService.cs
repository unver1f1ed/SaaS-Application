using SaaS_BLL.Common;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;
using SaaS_Domain.Enums;

namespace SaaS_BLL.Interfaces;

public interface IPaymentService
{
    Task<ServiceResult<PaymentDto>> GetByIdAsync(int id, CancellationToken ct = default);

    Task<ServiceResult<IEnumerable<PaymentDto>>> GetBySubscriptionIdAsync(int subscriptionId, CancellationToken ct = default);

    Task<ServiceResult<PaymentDto>> RecordPaymentAsync(RecordPaymentRequest request, CancellationToken ct = default);

    Task<ServiceResult<PaymentDto>> UpdateStatusAsync(int id, PaymentStatus status, CancellationToken ct = default);
}