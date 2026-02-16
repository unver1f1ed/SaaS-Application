using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaaS_Domain.Entities;

namespace SaaS_DAL.Configurations;

public class PaymentConfiguration : BaseEntityConfiguration<Payment>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(p => p.Amount)
            .HasColumnType("decimal(10,2)");

        builder.Property(p => p.Currency)
            .HasConversion<int>();

        builder.Property(p => p.PaymentMethod)
            .HasConversion<int>();

        builder.Property(p => p.Status)
            .HasConversion<int>();
    }
}