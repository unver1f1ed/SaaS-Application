// SaaS-BLL/Interfaces/IUserService.cs — add CreateAsync

using SaaS_BLL.Common;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;

namespace SaaS_BLL.Interfaces;

public interface IUserService
{
    Task<ServiceResult<UserDto>> GetByIdAsync(int id, CancellationToken ct = default);

    Task<ServiceResult<IEnumerable<UserDto>>> GetAllAsync(CancellationToken ct = default);

    Task<ServiceResult<UserDto>> CreateAsync(CreateUserRequest request, CancellationToken ct = default);  // NEW

    Task<ServiceResult<UserDto>> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct = default);

    Task<ServiceResult> DeleteAsync(int id, CancellationToken ct = default);
}