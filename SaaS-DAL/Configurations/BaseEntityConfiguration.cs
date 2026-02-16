using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaaS_Domain.Entities;

namespace SaaS_DAL.Configurations;

public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Configure primary key (common to all entities)
        builder.HasKey(e => e.Id);
        
        // Call derived class configuration
        ConfigureEntity(builder);
    }
    
    // Override this in derived classes for specific configuration
    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}