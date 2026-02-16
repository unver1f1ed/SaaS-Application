using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaaS_Domain.Entities;

namespace SaaS_DAL.Configurations;

public class SubscriptionConfiguration : BaseEntityConfiguration<Subscription>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Subscription> builder)
    {
        builder.Property(s => s.State)
            .HasConversion<int>();

        builder.Property(s => s.CancellationReason)
            .HasMaxLength(500);

        builder.HasMany(s => s.Payments)
            .WithOne(p => p.Subscription)
            .HasForeignKey(p => p.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.SubscriptionAddons)
            .WithOne(sa => sa.Subscription)
            .HasForeignKey(sa => sa.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}