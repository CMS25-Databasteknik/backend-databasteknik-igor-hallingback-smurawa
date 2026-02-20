using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.EFC.Configurations;

public sealed class VenueTypeEntityConfiguration : IEntityTypeConfiguration<VenueTypeEntity>
{
    public void Configure(EntityTypeBuilder<VenueTypeEntity> e)
    {
        var isSqliteTestMode = string.Equals(Environment.GetEnvironmentVariable("DB_PROVIDER"), "Sqlite", StringComparison.OrdinalIgnoreCase);

        e.ToTable("VenueTypes");

        e.HasKey(x => x.Id).HasName("PK_VenueTypes_Id");

        e.Property(x => x.Id)
            .ValueGeneratedNever();

        e.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        if (isSqliteTestMode)
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

        e.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_VenueTypes_Name");

        e.HasData(
            new VenueTypeEntity { Id = 1, Name = "InPerson", Concurrency = [0] },
            new VenueTypeEntity { Id = 2, Name = "Online", Concurrency = [0] },
            new VenueTypeEntity { Id = 3, Name = "Hybrid", Concurrency = [0] }
        );
    }
}
