using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.EFC.Configurations;

public sealed class ParticipantContactTypeEntityConfiguration : IEntityTypeConfiguration<ParticipantContactTypeEntity>
{
    public void Configure(EntityTypeBuilder<ParticipantContactTypeEntity> e)
    {
        e.ToTable("ParticipantContactTypes");

        e.HasKey(x => x.Id).HasName("PK_ParticipantContactTypes_Id");

        e.Property(x => x.Id)
            .ValueGeneratedNever();

        e.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        e.Property(x => x.Concurrency)
            .IsRowVersion()
            .IsConcurrencyToken()
            .IsRequired();

        e.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_ParticipantContactTypes_Name");

        e.HasData(
            new ParticipantContactTypeEntity { Id = 1, Name = "Primary", Concurrency = [0] },
            new ParticipantContactTypeEntity { Id = 2, Name = "Billing", Concurrency = [0] },
            new ParticipantContactTypeEntity { Id = 3, Name = "Emergency", Concurrency = [0] }
        );
    }
}
