using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.EFC.Configurations;

public sealed class InPlaceLocationEntityConfiguration : IEntityTypeConfiguration<InPlaceLocationEntity>
{
    public void Configure(EntityTypeBuilder<InPlaceLocationEntity> e)
    {
        var isSqliteTestMode = string.Equals(Environment.GetEnvironmentVariable("DB_PROVIDER"), "Sqlite", StringComparison.OrdinalIgnoreCase);

        e.ToTable("InPlaceLocations");

        e.HasKey(x => x.Id).HasName("PK_InPlaceLocations_Id");

        e.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        e.Property(x => x.RoomNumber)
            .IsRequired();

        e.Property(x => x.Seats)
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

        e.HasOne(ipl => ipl.Location)
            .WithMany(l => l.InPlaceLocations)
            .HasForeignKey(ipl => ipl.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        e.HasIndex(x => new { x.LocationId, x.RoomNumber })
            .IsUnique()
            .HasDatabaseName("IX_InPlaceLocations_LocationId_RoomNumber");
    }
}

