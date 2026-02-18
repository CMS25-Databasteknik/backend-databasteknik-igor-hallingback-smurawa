using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.EFC.Configurations;

public sealed class VenueTypeEntityConfiguration : IEntityTypeConfiguration<VenueTypeEntity>
{
    public void Configure(EntityTypeBuilder<VenueTypeEntity> e)
    {
        e.ToTable("VenueTypes");

        e.HasKey(x => x.Id).HasName("PK_VenueTypes_Id");

        e.Property(x => x.Id)
            .ValueGeneratedNever();

        e.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        e.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_VenueTypes_Name");

        e.HasData(
            new VenueTypeEntity { Id = 1, Name = "InPerson" },
            new VenueTypeEntity { Id = 2, Name = "Online" },
            new VenueTypeEntity { Id = 3, Name = "Hybrid" }
        );
    }
}
