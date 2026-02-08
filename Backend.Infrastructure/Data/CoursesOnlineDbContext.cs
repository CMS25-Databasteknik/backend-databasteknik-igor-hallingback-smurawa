using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Data
{
    public sealed class CoursesOnlineDbContext(DbContextOptions<CoursesOnlineDbContext> options) : DbContext(options)
    {
        public DbSet<CourseEntity> Courses => Set<CourseEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CourseEntity>(entity =>
            {
                entity.ToTable("Courses", tb =>
                    tb.HasCheckConstraint("CK_Courses_Title_NotEmpty", "LTRIM(RTRIM([Title])) <> ''"));

                entity.HasKey(e => e.Id).HasName("PK_Courses_Id");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("(NEWSEQUENTIALID())", "DF_Courses_Id");

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.Length)
                    .IsRequired();

                entity.Property(e => e.Concurrency)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .IsRequired();

                entity.Property(e => e.CreatedAtUtc)
                    .HasPrecision(0)
                    .HasDefaultValueSql("(SYSUTCDATETIME())", "DF_Courses_CreatedAtUtc")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.ModifiedAtUtc)
                    .HasPrecision(0)
                    .HasDefaultValueSql("(SYSUTCDATETIME())", "DF_Courses_ModifiedAtUtc")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasIndex(e => e.Title)
                    .HasDatabaseName("IX_Courses_Title");

                entity.HasMany(c => c.CourseEvents)
                    .WithOne(ce => ce.Course)
                    .HasForeignKey(ce => ce.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
