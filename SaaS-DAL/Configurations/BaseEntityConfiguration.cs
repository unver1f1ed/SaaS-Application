using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaaS_Domain.Entities;

namespace SaaS_DAL.Configurations;

/// <summary>
/// Base configuration class for entities using Fluent API.
/// Sets the primary key and allows derived classes to configure
/// additional entity-specific mappings.
/// </summary>
/// <typeparam name="TEntity">
/// The entity type being configured. Must inherit from <see cref="BaseEntity"/>.
/// </typeparam>
public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);
        
        ConfigureEntity(builder);
    }
    
    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}