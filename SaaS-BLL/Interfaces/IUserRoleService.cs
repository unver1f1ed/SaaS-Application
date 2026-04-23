using SaaS_BLL.Common;
using SaaS_BLL.Models;

namespace SaaS_BLL.Interfaces;

public interface IUserRoleService
{
    Task<ServiceResult<IEnumerable<UserRoleDto>>> GetAllAsync(CancellationToken ct = default);

    Task<ServiceResult<UserRoleDto>> GetByIdAsync(int id, CancellationToken ct = default);
}