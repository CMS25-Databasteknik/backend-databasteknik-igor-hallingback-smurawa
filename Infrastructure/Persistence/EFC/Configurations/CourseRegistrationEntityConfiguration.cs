using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.EFC.Configurations;

public sealed class CourseRegistrationEntityConfiguration : IEntityTypeConfiguration<CourseRegistrationEntity>
{
    public void Configure(EntityTypeBuilder<CourseRegistrationEntity> e)
    {
        e.ToTable("CourseRegistrations");

        e.HasKey(x => x.Id).HasName("PK_CourseRegistrations_Id");

        e.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("(NEWSEQUENTIALID())", "DF_CourseRegistrations_Id");

        e.Property(x => x.RegistrationDate)
            .HasPrecision(0)
            .HasDefaultValueSql("(SYSUTCDATETIME())", "DF_CourseRegistrations_RegistrationDate")
            .ValueGeneratedOnAdd();

        e.Property(x => x.IsPaid)
            .HasDefaultValue(false)
            .IsRequired();

        e.Property(x => x.Concurrency)
            .IsRowVersion()
            .IsConcurrencyToken()
            .IsRequired();

        e.Property(x => x.ModifiedAtUtc)
            .HasPrecision(0)
            .HasDefaultValueSql("(SYSUTCDATETIME())", "DF_CourseRegistrations_ModifiedAtUtc")
            .ValueGeneratedOnAddOrUpdate();

        e.HasIndex(x => new { x.ParticipantId, x.CourseEventId })
            .IsUnique()
            .HasDatabaseName("IX_CourseRegistrations_ParticipantId_CourseEventId");

        e.HasOne(cr => cr.Participant)
            .WithMany(p => p.CourseRegistrations)
            .HasForeignKey(cr => cr.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(cr => cr.CourseEvent)
            .WithMany(ce => ce.Registrations)
            .HasForeignKey(cr => cr.CourseEventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

