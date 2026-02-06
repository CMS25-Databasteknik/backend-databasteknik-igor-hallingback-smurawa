using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Data
{
    public sealed class MemblerDbContext(DbContextOptions<MemblerDbContext> options) : DbContext(options)
    {
        public DbSet<MemberEntity> Members => Set<MemberEntity>();
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

                entity.HasIndex(e => e.Email, "UQ_Members_Email").IsUnique();

                entity.ToTable(tb => tb.HasCheckConstraint("CK_Members_Email_NotEmpty", "LTRIM(RTRIM([Email])) <> ''"));

            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
