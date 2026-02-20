using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Hosting;

namespace Backend.Infrastructure.Persistence.EFC.Configurations;

public sealed class CourseEntityConfiguration : IEntityTypeConfiguration<CourseEntity>
{
    public void Configure(EntityTypeBuilder<CourseEntity> e)
    {
        var isDevelopment = string.Equals(
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            Environments.Development,
            StringComparison.OrdinalIgnoreCase);

        e.ToTable("Courses", t =>
        {
            t.HasCheckConstraint("CK_Courses_Title_NotEmpty", "LTRIM(RTRIM([Title])) <> ''");
        });

        e.HasKey(x => x.Id).HasName("PK_Courses_Id");

        var idProperty = e.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        if (!isDevelopment)
        {
            idProperty.HasDefaultValueSql("(NEWSEQUENTIALID())", "DF_Courses_Id");
        }

        e.Property(x => x.Title)
            .HasMaxLength(100)
            .IsRequired();

        e.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();

        e.Property(x => x.DurationInDays)
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

        var createdAtProperty = e.Property(x => x.CreatedAtUtc)
            .HasPrecision(0)
            .ValueGeneratedOnAdd();

        var modifiedAtProperty = e.Property(x => x.ModifiedAtUtc)
            .HasPrecision(0)
            .ValueGeneratedOnAddOrUpdate();

        if (isDevelopment)
        {
            createdAtProperty.HasDefaultValueSql("(CURRENT_TIMESTAMP)");
            modifiedAtProperty.HasDefaultValueSql("(CURRENT_TIMESTAMP)");
        }
        else
        {
            createdAtProperty.HasDefaultValueSql("(SYSUTCDATETIME())", "DF_Courses_CreatedAtUtc");
            modifiedAtProperty.HasDefaultValueSql("(SYSUTCDATETIME())", "DF_Courses_ModifiedAtUtc");
        }

        e.HasIndex(x => x.Title)
            .HasDatabaseName("IX_Courses_Title");

        e.HasMany(c => c.CourseEvents)
            .WithOne(ce => ce.Course)
            .HasForeignKey(ce => ce.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

