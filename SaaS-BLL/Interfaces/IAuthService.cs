using SaaS_BLL.Common;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;

namespace SaaS_BLL.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<UserDto>> LoginAsync(LoginRequest request, CancellationToken ct = default);

    Task<ServiceResult<UserDto>> RegisterAsync(RegisterRequest request, CancellationToken ct = default);

    Task<ServiceResult> ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken ct = default);
}