namespace SaaS_BLL.Models;

public record UserRoleDto(
    int Id,
    string RoleName,
    string? Description);