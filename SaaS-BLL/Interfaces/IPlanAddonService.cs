using SaaS_BLL.Common;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;

namespace SaaS_BLL.Interfaces;

public interface IPlanAddonService
{
    Task<ServiceResult<IEnumerable<PlanAddonDto>>> GetByPlanIdAsync(int planId, CancellationToken ct = default);

    Task<ServiceResult<PlanAddonDto>> GetByIdAsync(int id, CancellationToken ct = default);

    Task<ServiceResult<PlanAddonDto>> CreateAsync(CreatePlanAddonRequest request, CancellationToken ct = default);

    Task<ServiceResult<PlanAddonDto>> UpdateAsync(int id, UpdatePlanAddonRequest request, CancellationToken ct = default);

    Task<ServiceResult> DeleteAsync(int id, CancellationToken ct = default);
}