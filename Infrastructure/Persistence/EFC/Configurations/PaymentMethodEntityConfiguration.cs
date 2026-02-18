using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.EFC.Configurations;

public sealed class PaymentMethodEntityConfiguration : IEntityTypeConfiguration<PaymentMethodEntity>
{
    public void Configure(EntityTypeBuilder<PaymentMethodEntity> e)
    {
        e.ToTable("PaymentMethods");

        e.HasKey(x => x.Id).HasName("PK_PaymentMethods_Id");

        e.Property(x => x.Id)
            .ValueGeneratedNever();

        e.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        e.Property(x => x.Concurrency)
            .IsRowVersion()
            .IsConcurrencyToken()
            .IsRequired();

        e.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_PaymentMethods_Name");

        e.HasData(
            new PaymentMethodEntity { Id = 1, Name = "Card" },
            new PaymentMethodEntity { Id = 2, Name = "Invoice" },
            new PaymentMethodEntity { Id = 3, Name = "Cash" }
        );
    }
}
