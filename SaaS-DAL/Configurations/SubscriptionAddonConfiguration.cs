using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaaS_Domain.Entities;

namespace SaaS_DAL.Configurations;

public class SubscriptionAddonConfiguration : BaseEntityConfiguration<SubscriptionAddon>
{
    protected override void ConfigureEntity(EntityTypeBuilder<SubscriptionAddon> builder)
    {
        builder.Property(sa => sa.Quantity)
            .HasDefaultValue(1);
    }
}