using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Hosting;

namespace Backend.Infrastructure.Persistence.EFC.Configurations;

public sealed class LocationEntityConfiguration : IEntityTypeConfiguration<LocationEntity>
{
    public void Configure(EntityTypeBuilder<LocationEntity> e)
    {
        var isDevelopment = string.Equals(
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            Environments.Development,
            StringComparison.OrdinalIgnoreCase);

        e.ToTable("Locations", t =>
        {
            t.HasCheckConstraint("CK_Locations_PostalCode_NotEmpty", "LTRIM(RTRIM([PostalCode])) <> ''");
        });

        e.HasKey(x => x.Id).HasName("PK_Locations_Id");

        e.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        e.Property(x => x.StreetName)
            .HasMaxLength(50)
            .IsRequired();

        e.Property(x => x.PostalCode)
            .HasMaxLength(6)
            .IsUnicode(false)
            .IsRequired();

        e.Property(x => x.City)
            .HasMaxLength(50)
            .IsRequired();

        if (isDevelopment)
        {
            e.Property(x => x.Concurrency)
                .IsConcurrencyToken()
                .IsRequired(false);
        }
        else
        {
            e.Property(x => x.Concurrency)
                .IsRowVersion()
                .IsConcurrencyToken()
                .IsRequired();
        }

        e.HasIndex(x => x.PostalCode)
            .HasDatabaseName("IX_Locations_PostalCode");
    }
}

