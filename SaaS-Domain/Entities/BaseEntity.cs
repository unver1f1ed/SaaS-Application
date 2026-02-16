using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS_Domain.Entities;

public abstract class BaseEntity
{
    protected BaseEntity(int id)
    {
        this.Id = id;
    }
    protected BaseEntity()
    {
        this.Id = 0;
    }
    public int Id { get; set; }
}