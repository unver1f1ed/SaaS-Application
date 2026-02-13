using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS_Domain.Entities;

[Table("users")]
public class User : BaseEntity
{
    public User()
        : base()
    {
    }

    public User(int id, string firstName, string lastName, string email, 
        string passwordHash, int roleId)
        : base(id)
    {
        this.FirstName =  firstName;
        this.LastName = lastName;
        this.Email = email;
        this.PasswordHash = passwordHash;
        this.UserRoleId = roleId;
    }
    
    [Column("first_name")] public string FirstName { get; set; } = null!;
    
    [Column("last_name")] public string LastName { get; set; } = null!;

    [Column("email")] public string Email { get; set; } = null!;

    [Column("password_hash")] public string PasswordHash { get; set; } = null!;
    
    [Column("user_role_id")] public int UserRoleId { get; set; }

    public UserRole UserRole { get; set; } = null!;
    
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}