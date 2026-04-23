using AutoMapper;
using SaaS_BLL.Common;
using SaaS_BLL.Interfaces;
using SaaS_BLL.Models;
using SaaS_Domain.Interfaces;

namespace SaaS_BLL.Services;

public class UserRoleService : IUserRoleService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UserRoleService(IUnitOfWork uow, IMapper mapper)
    {
        this._uow = uow;
        this._mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<UserRoleDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var roles = await this._uow.UserRoles.GetAllAsync(ct);
        return ServiceResult<IEnumerable<UserRoleDto>>.Ok(this._mapper.Map<IEnumerable<UserRoleDto>>(roles));
    }

    public async Task<ServiceResult<UserRoleDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var role = await this._uow.UserRoles.GetByIdAsync(id, ct);
        return role is null ? ServiceResult<UserRoleDto>.Fail("Role not found.") : ServiceResult<UserRoleDto>.Ok(this._mapper.Map<UserRoleDto>(role));
    }
}