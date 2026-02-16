using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaaS_Domain.Entities;

namespace SaaS_DAL.Configurations;

public class PlanConfiguration : BaseEntityConfiguration<Plan>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Plan> builder)
    {
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.BasePrice)
            .HasColumnType("decimal(10,2)");

        builder.Property(p => p.PlanType)
            .HasConversion<int>();

        builder.Property(p => p.Currency)
            .HasConversion<int>();

        builder.Property(p => p.BillingInterval)
            .HasConversion<int>();

        builder.HasMany(p => p.Subscriptions)
            .WithOne(s => s.Plan)
            .HasForeignKey(s => s.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.PlanAddons)
            .WithOne(pa => pa.Plan)
            .HasForeignKey(pa => pa.PlanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}