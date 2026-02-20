using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Hosting;

namespace Backend.Infrastructure.Persistence.EFC.Configurations;

public sealed class CourseEventEntityConfiguration : IEntityTypeConfiguration<CourseEventEntity>
{
    public void Configure(EntityTypeBuilder<CourseEventEntity> e)
    {
        var isDevelopment = string.Equals(
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            Environments.Development,
            StringComparison.OrdinalIgnoreCase);

        e.ToTable("CourseEvents", t =>
        {
            t.HasCheckConstraint("CK_CourseEvents_Price", "[Price] >= 0");
            t.HasCheckConstraint("CK_CourseEvents_Seats", "[Seats] > 0");
        });

        e.HasKey(x => x.Id).HasName("PK_CourseEvents_Id");

        e.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("(NEWSEQUENTIALID())", "DF_CourseEvents_Id");

        e.Property(x => x.EventDate)
            .HasPrecision(0)
            .IsRequired();

        e.Property(x => x.Price)
            .HasColumnType("money")
            .IsRequired();

        e.Property(x => x.Seats)
            .IsRequired();

        e.Property(x => x.VenueTypeId)
            .HasDefaultValue(1)
            .IsRequired();

        if (isDevelopment)
        {
            e.Property(x => x.Concurrency)
                .IsConcurrencyToken()
                .IsRequired(false);

            e.Property(x => x.CreatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();

            e.Property(x => x.ModifiedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAddOrUpdate();
        }
        else
        {
            e.Property(x => x.Concurrency)
                .IsRowVersion()
                .IsConcurrencyToken()
                .IsRequired();

            e.Property(x => x.CreatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(SYSUTCDATETIME())", "DF_CourseEvents_CreatedAtUtc")
                .ValueGeneratedOnAdd();

            e.Property(x => x.ModifiedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(SYSUTCDATETIME())", "DF_CourseEvents_ModifiedAtUtc")
                .ValueGeneratedOnAddOrUpdate();
        }

        e.HasIndex(x => new { x.CourseId, x.EventDate })
            .HasDatabaseName("IX_CourseEvents_CourseId_EventDate");

        e.HasOne(ce => ce.CourseEventType)
            .WithMany(cet => cet.CourseEvents)
            .HasForeignKey(ce => ce.CourseEventTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(ce => ce.VenueType)
            .WithMany()
            .HasForeignKey(ce => ce.VenueTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        e.HasMany(ce => ce.Instructors)
            .WithMany(i => i.CourseEvents)
            .UsingEntity<Dictionary<string, object>>(
                "CourseEventInstructors",
                i => i.HasOne<InstructorEntity>().WithMany().HasForeignKey("InstructorId").OnDelete(DeleteBehavior.Cascade),
                ce => ce.HasOne<CourseEventEntity>().WithMany().HasForeignKey("CourseEventId").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("CourseEventId", "InstructorId");
                    j.ToTable("CourseEventInstructors");
                });

        e.HasMany(ce => ce.InPlaceLocations)
            .WithMany(ipl => ipl.CourseEvents)
            .UsingEntity<Dictionary<string, object>>(
                "InPlaceEventLocations",
                ipl => ipl.HasOne<InPlaceLocationEntity>().WithMany().HasForeignKey("InPlaceLocationId").OnDelete(DeleteBehavior.Cascade),
                ce => ce.HasOne<CourseEventEntity>().WithMany().HasForeignKey("CourseEventId").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("CourseEventId", "InPlaceLocationId");
                    j.ToTable("InPlaceEventLocations");
                });
    }
}

