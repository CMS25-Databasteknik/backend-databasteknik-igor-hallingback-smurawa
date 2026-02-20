using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.EFC.Configurations;

public sealed class CourseRegistrationStatusEntityConfiguration : IEntityTypeConfiguration<CourseRegistrationStatusEntity>
{
    public void Configure(EntityTypeBuilder<CourseRegistrationStatusEntity> e)
    {
        var isSqliteTestMode = string.Equals(Environment.GetEnvironmentVariable("DB_PROVIDER"), "Sqlite", StringComparison.OrdinalIgnoreCase);

        e.ToTable("CourseRegistrationStatuses");

        e.HasKey(x => x.Id).HasName("PK_CourseRegistrationStatuses_Id");

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
            .HasDatabaseName("IX_CourseRegistrationStatuses_Name");

        e.HasData(
            new CourseRegistrationStatusEntity { Id = 0, Name = "Pending", Concurrency = [0] },
            new CourseRegistrationStatusEntity { Id = 1, Name = "Paid", Concurrency = [0] },
            new CourseRegistrationStatusEntity { Id = 2, Name = "Cancelled", Concurrency = [0] },
            new CourseRegistrationStatusEntity { Id = 3, Name = "Refunded", Concurrency = [0] }
        );
    }
}
