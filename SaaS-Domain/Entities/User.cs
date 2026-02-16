using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS_Domain.Entities;

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
    
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public int UserRoleId { get; set; }

    public UserRole UserRole { get; set; } = null!;
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}