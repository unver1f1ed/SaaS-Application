using SaaS_BLL.Common;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;

namespace SaaS_BLL.Interfaces;

public interface ISubscriptionAddonService
{
    Task<ServiceResult<IEnumerable<SubscriptionAddonDto>>> GetBySubscriptionIdAsync(int subscriptionId, CancellationToken ct = default);

    Task<ServiceResult<SubscriptionAddonDto>> AddAddonAsync(AddSubscriptionAddonRequest request, CancellationToken ct = default);

    Task<ServiceResult> RemoveAddonAsync(int id, CancellationToken ct = default);
}