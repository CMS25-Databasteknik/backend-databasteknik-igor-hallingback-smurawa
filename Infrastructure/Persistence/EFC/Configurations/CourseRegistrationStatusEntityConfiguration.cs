using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.EFC.Configurations;

public sealed class CourseRegistrationStatusEntityConfiguration : IEntityTypeConfiguration<CourseRegistrationStatusEntity>
{
    public void Configure(EntityTypeBuilder<CourseRegistrationStatusEntity> e)
    {
        e.ToTable("CourseRegistrationStatuses");

        e.HasKey(x => x.Id).HasName("PK_CourseRegistrationStatuses_Id");

        e.Property(x => x.Id)
            .ValueGeneratedNever();

        e.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        e.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_CourseRegistrationStatuses_Name");

        e.HasData(
            new CourseRegistrationStatusEntity { Id = 0, Name = "Pending" },
            new CourseRegistrationStatusEntity { Id = 1, Name = "Paid" },
            new CourseRegistrationStatusEntity { Id = 2, Name = "Cancelled" },
            new CourseRegistrationStatusEntity { Id = 3, Name = "Refunded" }
        );
    }
}
