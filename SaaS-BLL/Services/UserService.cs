using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SaaS_BLL.Common;
using SaaS_BLL.Interfaces;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;
using SaaS_Domain.Entities;
using SaaS_Domain.Interfaces;

namespace SaaS_BLL.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork uow, IMapper mapper)
    {
        this._uow = uow;
        this._mapper = mapper;
    }

    public async Task<ServiceResult<UserDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var user = await this._uow.Users.Query()
            .Include(u => u.UserRole)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        return user is null ? ServiceResult<UserDto>.Fail("User not found.") : ServiceResult<UserDto>.Ok(this._mapper.Map<UserDto>(user));
    }

    public async Task<ServiceResult<IEnumerable<UserDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var users = await this._uow.Users.Query()
            .Include(u => u.UserRole)
            .ToListAsync(ct);

        return ServiceResult<IEnumerable<UserDto>>.Ok(this._mapper.Map<IEnumerable<UserDto>>(users));
    }

    public async Task<ServiceResult<UserDto>> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            return ServiceResult<UserDto>.Fail("First and last name are required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            return ServiceResult<UserDto>.Fail("Password must be at least 8 characters.");
        }

        var emailNormalized = request.Email.ToLower().Trim();

        var emailTaken = await this._uow.Users.Query()
            .AnyAsync(u => u.Email == emailNormalized, ct);

        if (emailTaken)
        {
            return ServiceResult<UserDto>.Fail("An account with this email already exists.");
        }

        var roleExists = await this._uow.UserRoles.ExistsAsync(request.UserRoleId, ct);
        if (!roleExists)
        {
            return ServiceResult<UserDto>.Fail("Role not found.");
        }

        var user = new User(
            id: 0,
            firstName: request.FirstName.Trim(),
            lastName: request.LastName.Trim(),
            email: emailNormalized,
            passwordHash: BCrypt.Net.BCrypt.HashPassword(request.Password),
            roleId: request.UserRoleId);

        await this._uow.Users.AddAsync(user, ct);
        await this._uow.SaveChangesAsync(ct);

        return await this.GetByIdAsync(user.Id, ct);
    }

    public async Task<ServiceResult<UserDto>> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var user = await this._uow.Users.GetByIdAsync(id, ct);
        if (user is null)
        {
            return ServiceResult<UserDto>.Fail("User not found.");
        }

        var emailNormalized = request.Email.ToLower().Trim();

        var emailTaken = await this._uow.Users.Query()
            .AnyAsync(u => u.Email == emailNormalized && u.Id != id, ct);

        if (emailTaken)
        {
            return ServiceResult<UserDto>.Fail("Email is already in use.");
        }

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Email = emailNormalized;

        this._uow.Users.Update(user);
        await this._uow.SaveChangesAsync(ct);

        return await this.GetByIdAsync(id, ct);
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken ct = default)
    {
        var user = await this._uow.Users.GetByIdAsync(id, ct);
        if (user is null)
        {
            return ServiceResult.Fail("User not found.");
        }

        this._uow.Users.Delete(user);
        await this._uow.SaveChangesAsync(ct);

        return ServiceResult.Ok();
    }
}