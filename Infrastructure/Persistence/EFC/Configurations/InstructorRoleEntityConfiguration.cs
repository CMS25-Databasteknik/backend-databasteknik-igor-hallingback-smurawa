using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.EFC.Configurations;

public sealed class InstructorRoleEntityConfiguration : IEntityTypeConfiguration<InstructorRoleEntity>
{
    public void Configure(EntityTypeBuilder<InstructorRoleEntity> e)
    {
        e.ToTable("InstructorRoles", t =>
        {
            t.HasCheckConstraint("CK_InstructorRoles_RoleName_NotEmpty", "LEN([RoleName]) > 0");
        });

        e.HasKey(x => x.Id).HasName("PK_InstructorRoles_Id");

        e.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        e.Property(x => x.RoleName)
            .HasMaxLength(50)
            .IsRequired();

        e.HasIndex(x => x.RoleName)
            .IsUnique()
            .HasDatabaseName("IX_InstructorRoles_RoleName");
    }
}
