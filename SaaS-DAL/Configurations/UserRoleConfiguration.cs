using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaaS_Domain.Entities;

namespace SaaS_DAL.Configurations;

public class UserRoleConfiguration : BaseEntityConfiguration<UserRole>
{
    protected override void ConfigureEntity(EntityTypeBuilder<UserRole> builder)
    {
        builder.Property(ur => ur.RoleName)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(ur => ur.RoleName)
            .IsUnique();

        builder.Property(ur => ur.Description)
            .HasMaxLength(255);

        builder.HasMany(ur => ur.Users)
            .WithOne(u => u.UserRole)
            .HasForeignKey(u => u.UserRoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}