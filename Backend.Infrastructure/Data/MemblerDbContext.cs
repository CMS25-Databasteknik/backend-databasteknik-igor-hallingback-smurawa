using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Data
{
    public sealed class MemblerDbContext(DbContextOptions<MemblerDbContext> options) : DbContext(options)
    {
        public DbSet<MemberEntity> Members => Set<MemberEntity>();
        public DbSet<RoleEntity> Roles => Set<RoleEntity>();
        public DbSet<StatusTypeEntity> StatusTypes => Set<StatusTypeEntity>();
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MemberEntity>(entity =>
            {
                entity.ToTable("Members");

                entity.HasKey(e => e.Id).HasName("PK_Members_Id");

                entity.Property(entity => entity.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("(NEWSEQUENTIALID())", "DF_Members_Id");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.Email)
                    .HasMaxLength(256)
                    .IsRequired();

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(123)
                    .IsUnicode(false)
                    .IsRequired(false);

                entity.Property(e => e.Concurrency)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .IsRequired();

                entity.Property(e => e.CreatedAtUtc)
                    .HasPrecision(0) // datetime2(0) 14:35:15.0123
                    .HasDefaultValueSql("(SYSUTCDATETIME())", "DF_Members_CreatedAtUtc")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.ModifiedAtUtc)
                    .HasPrecision(0) // datetime2(0) 14:35:15.0123
                    .HasDefaultValueSql("(SYSUTCDATETIME())", "DF_Members_ModifiedAtUtc")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.CurrentStatusId)
                    .IsRequired();

                entity.HasIndex(e => e.Email, "UQ_Members_Email").IsUnique();

                entity.ToTable(tb => tb.HasCheckConstraint("CK_Members_Email_NotEmpty", "LTRIM(RTRIM([Email])) <> ''"));

                entity.HasOne(e => e.CurrentStatus)
                    .WithMany(s => s.Members)
                    .HasForeignKey(e => e.CurrentStatusId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Members_StatusTypes_CurrentStatusId");

            });

            modelBuilder.Entity<RoleEntity>(entity =>
            {
                entity.ToTable("Roles");

                entity.HasKey(e => e.Id).HasName("PK_Roles_Id");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.HasIndex(e => e.RoleName, "UQ_Roles_RoleName")
                    .IsUnique();

            });

            modelBuilder.Entity<MemberEntity>()
                .HasMany(m => m.Roles)
                .WithMany(r => r.Members)
                .UsingEntity<Dictionary<string, object>>(
                    "MemberRole",
                    r => r.HasOne<RoleEntity>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.ClientSetNull),
                    m => m.HasOne<MemberEntity>().WithMany().HasForeignKey("MemberId").OnDelete(DeleteBehavior.ClientSetNull),
                    e =>
                    {
                        e.HasKey("MemberId", "RoleId");
                        e.ToTable("MemberRoles");
                    }
                );

            modelBuilder.Entity<StatusTypeEntity>(entity =>
            {
                entity.ToTable("StatusTypes");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.StatusName)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.HasIndex(e => e.StatusName, "UQ_StatusTypes_StatusName")
                    .IsUnique();
            });


        }
    }
}
