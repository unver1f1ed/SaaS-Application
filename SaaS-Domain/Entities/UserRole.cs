using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS_Domain.Entities;

[Table("user_roles")]
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

    [Column("role_name")] public string RoleName { get; set; } = null!;
    
    [Column("description")] public string? Description { get; set; }
    
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}