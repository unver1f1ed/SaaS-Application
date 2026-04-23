using System.Net.Mail;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SaaS_BLL.Common;
using SaaS_BLL.Interfaces;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;
using SaaS_Domain.Entities;
using SaaS_Domain.Interfaces;

namespace SaaS_BLL.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AuthService(IUnitOfWork uow, IMapper mapper)
    {
        this._uow = uow;
        this._mapper = mapper;
    }

    public async Task<ServiceResult<UserDto>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return ServiceResult<UserDto>.Fail("Email and password are required.");
        }

        var user = await this._uow.Users.Query()
            .Where(u => u.Email == request.Email.ToLower().Trim())
            .Select(u => new { u.Id, u.Email, u.PasswordHash, u.UserRoleId })
            .FirstOrDefaultAsync(ct);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return ServiceResult<UserDto>.Fail("Invalid email or password.");
        }

        // Re-fetch with role for mapping
        var fullUser = await this._uow.Users.Query()
            .Include(u => u.UserRole)
            .FirstAsync(u => u.Id == user.Id, ct);

        return ServiceResult<UserDto>.Ok(this._mapper.Map<UserDto>(fullUser));
    }

    public async Task<ServiceResult<UserDto>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            return ServiceResult<UserDto>.Fail("First and last name are required.");
        }

        if (!IsValidEmail(request.Email))
        {
            return ServiceResult<UserDto>.Fail("Invalid email format.");
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

        // Default role: User (Id = 2 per seed data)
        const int defaultRoleId = 2;

        var user = new User(
            id: 0,
            firstName: request.FirstName.Trim(),
            lastName: request.LastName.Trim(),
            email: emailNormalized,
            passwordHash: BCrypt.Net.BCrypt.HashPassword(request.Password),
            roleId: defaultRoleId);

        await this._uow.Users.AddAsync(user, ct);
        await this._uow.SaveChangesAsync(ct);

        var created = await this._uow.Users.Query()
            .Include(u => u.UserRole)
            .FirstAsync(u => u.Id == user.Id, ct);

        return ServiceResult<UserDto>.Ok(this._mapper.Map<UserDto>(created));
    }

    public async Task<ServiceResult> ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 8)
        {
            return ServiceResult.Fail("New password must be at least 8 characters.");
        }

        var user = await this._uow.Users.GetByIdAsync(userId, ct);
        if (user is null)
        {
            return ServiceResult.Fail("User not found.");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return ServiceResult.Fail("Current password is incorrect.");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        this._uow.Users.Update(user);
        await this._uow.SaveChangesAsync(ct);

        return ServiceResult.Ok();
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}