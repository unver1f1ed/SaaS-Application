namespace SaaS_BLL.Models;

public record UserDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    int UserRoleId,
    string RoleName);