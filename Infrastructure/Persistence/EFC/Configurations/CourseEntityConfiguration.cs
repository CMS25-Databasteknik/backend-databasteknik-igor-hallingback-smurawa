using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.EFC.Configurations;

public sealed class CourseEntityConfiguration : IEntityTypeConfiguration<CourseEntity>
{
    public void Configure(EntityTypeBuilder<CourseEntity> e)
    {
        e.ToTable("Courses", t =>
        {
            t.HasCheckConstraint("CK_Courses_Title_NotEmpty", "LTRIM(RTRIM([Title])) <> ''");
        });

        e.HasKey(x => x.Id).HasName("PK_Courses_Id");

        e.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("(NEWSEQUENTIALID())", "DF_Courses_Id");

        e.Property(x => x.Title)
            .HasMaxLength(100)
            .IsRequired();

        e.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();

        e.Property(x => x.DurationInDays)
            .IsRequired();

        e.Property(x => x.Concurrency)
            .IsRowVersion()
            .IsConcurrencyToken()
            .IsRequired();

        e.Property(x => x.CreatedAtUtc)
            .HasPrecision(0)
            .HasDefaultValueSql("(SYSUTCDATETIME())", "DF_Courses_CreatedAtUtc")
            .ValueGeneratedOnAdd();

        e.Property(x => x.ModifiedAtUtc)
            .HasPrecision(0)
            .HasDefaultValueSql("(SYSUTCDATETIME())", "DF_Courses_ModifiedAtUtc")
            .ValueGeneratedOnAddOrUpdate();

        e.HasIndex(x => x.Title)
            .HasDatabaseName("IX_Courses_Title");

        e.HasMany(c => c.CourseEvents)
            .WithOne(ce => ce.Course)
            .HasForeignKey(ce => ce.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

