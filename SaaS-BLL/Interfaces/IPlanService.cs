using SaaS_BLL.Common;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;

namespace SaaS_BLL.Interfaces;

public interface IPlanService
{
    Task<ServiceResult<IEnumerable<PlanDto>>> GetAllAsync(CancellationToken ct = default);

    Task<ServiceResult<PlanDto>> GetByIdAsync(int id, CancellationToken ct = default);

    Task<ServiceResult<PlanDto>> CreateAsync(CreatePlanRequest request, CancellationToken ct = default);

    Task<ServiceResult<PlanDto>> UpdateAsync(int id, UpdatePlanRequest request, CancellationToken ct = default);

    Task<ServiceResult> DeleteAsync(int id, CancellationToken ct = default);
}