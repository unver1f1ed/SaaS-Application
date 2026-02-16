using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaaS_Domain.Entities;

namespace SaaS_DAL.Configurations;

public class PlanAddonConfiguration : BaseEntityConfiguration<PlanAddon>
{
    protected override void ConfigureEntity(EntityTypeBuilder<PlanAddon> builder)
    {
        builder.Property(pa => pa.AddonName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pa => pa.Description)
            .HasMaxLength(500);

        builder.Property(pa => pa.UnitPrice)
            .HasColumnType("decimal(10,2)");

        builder.Property(pa => pa.Currency)
            .HasConversion<int>();

        builder.Property(pa => pa.BillingType)
            .HasConversion<int>();

        builder.HasMany(pa => pa.SubscriptionAddons)
            .WithOne(sa => sa.PlanAddon)
            .HasForeignKey(sa => sa.PlanAddonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}