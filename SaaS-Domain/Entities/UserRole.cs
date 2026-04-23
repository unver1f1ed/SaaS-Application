namespace SaaS_Domain.Entities;

public class UserRole : BaseEntity
{
    public UserRole()
    {
    }

    public UserRole(int id, string roleName, string? description)
        : base(id)
    {
        this.RoleName = roleName;
        this.Description = description;
    }

    public string RoleName { get; init; } = null!;

    public string? Description { get; init; }

    public virtual ICollection<User> Users { get; init; } = new List<User>();
}