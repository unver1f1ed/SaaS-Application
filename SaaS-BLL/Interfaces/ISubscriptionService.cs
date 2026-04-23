using SaaS_BLL.Common;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;

namespace SaaS_BLL.Interfaces;

public interface ISubscriptionService
{
    Task<ServiceResult<IEnumerable<SubscriptionDto>>> GetAllAsync(CancellationToken ct = default);

    Task<ServiceResult<SubscriptionDto>> GetByIdAsync(int id, CancellationToken ct = default);

    Task<ServiceResult<IEnumerable<SubscriptionDto>>> GetByUserIdAsync(int userId, CancellationToken ct = default);

    Task<ServiceResult<SubscriptionDto>> SubscribeAsync(CreateSubscriptionRequest request, CancellationToken ct = default);

    Task<ServiceResult<SubscriptionDto>> UpdateAsync(int id, UpdateSubscriptionRequest request, CancellationToken ct = default);

    Task<ServiceResult<SubscriptionDto>> CancelAsync(int id, CancelSubscriptionRequest request, CancellationToken ct = default);

    Task<ServiceResult<SubscriptionDto>> RenewAsync(int id, CancellationToken ct = default);

    Task<ServiceResult> DeleteAsync(int id, CancellationToken ct = default);
}