using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Hosting;

namespace Backend.Infrastructure.Persistence.EFC.Configurations;

public sealed class CourseEventTypeEntityConfiguration : IEntityTypeConfiguration<CourseEventTypeEntity>
{
    public void Configure(EntityTypeBuilder<CourseEventTypeEntity> e)
    {
        var isDevelopment = string.Equals(
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            Environments.Development,
            StringComparison.OrdinalIgnoreCase);

        e.ToTable("CourseEventTypes", t =>
        {
            t.HasCheckConstraint("CK_CourseEventTypes_TypeName_NotEmpty", "LTRIM(RTRIM([TypeName])) <> ''");
        });

        e.HasKey(x => x.Id).HasName("PK_CourseEventTypes_Id");

        e.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        e.Property(x => x.TypeName)
            .HasMaxLength(20)
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

        e.HasIndex(x => x.TypeName)
            .IsUnique()
            .HasDatabaseName("IX_CourseEventTypes_TypeName");
    }
}

