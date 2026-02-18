using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.EFC.Configurations;

public sealed class CourseEventTypeEntityConfiguration : IEntityTypeConfiguration<CourseEventTypeEntity>
{
    public void Configure(EntityTypeBuilder<CourseEventTypeEntity> e)
    {
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

        e.Property(x => x.Concurrency)
            .IsRowVersion()
            .IsConcurrencyToken()
            .IsRequired();

        e.HasIndex(x => x.TypeName)
            .IsUnique()
            .HasDatabaseName("IX_CourseEventTypes_TypeName");
    }
}

