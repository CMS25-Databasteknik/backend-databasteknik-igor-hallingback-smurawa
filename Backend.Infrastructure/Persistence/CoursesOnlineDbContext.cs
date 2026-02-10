using Backend.Infrastructure.Persistence.EFC.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence
{
    public sealed class CoursesOnlineDbContext(DbContextOptions<CoursesOnlineDbContext> options) : DbContext(options)
    {
        public DbSet<CourseEntity> Courses => Set<CourseEntity>();
        public DbSet<CourseEventEntity> CourseEvents => Set<CourseEventEntity>();
        public DbSet<CourseEventTypeEntity> CourseEventTypes => Set<CourseEventTypeEntity>();
        public DbSet<InstructorEntity> Instructors => Set<InstructorEntity>();
        public DbSet<LocationEntity> Locations => Set<LocationEntity>();
        public DbSet<ParticipantEntity> Participants => Set<ParticipantEntity>();
        public DbSet<InPlaceLocationEntity> InPlaceLocations => Set<InPlaceLocationEntity>();
        public DbSet<CourseRegistrationEntity> CourseRegistrations => Set<CourseRegistrationEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ========================================
            // INDEPENDENT ENTITIES (No foreign keys)
            // ========================================

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

                entity.Property(e => e.DurationInDays)
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

            modelBuilder.Entity<CourseEventTypeEntity>(entity =>
            {
                entity.ToTable("CourseEventTypes", tb =>
                    tb.HasCheckConstraint("CK_CourseEventTypes_TypeName_NotEmpty", "LTRIM(RTRIM([TypeName])) <> ''"));

                entity.HasKey(e => e.Id).HasName("PK_CourseEventTypes_Id");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.TypeName)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.HasIndex(e => e.TypeName)
                    .IsUnique()
                    .HasDatabaseName("IX_CourseEventTypes_TypeName");
            });

            modelBuilder.Entity<InstructorEntity>(entity =>
            {
                entity.ToTable("Instructors", tb =>
                    tb.HasCheckConstraint("CK_Instructors_Name_NotEmpty", "LTRIM(RTRIM([Name])) <> ''"));

                entity.HasKey(e => e.Id).HasName("PK_Instructors_Id");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("(NEWSEQUENTIALID())", "DF_Instructors_Id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("IX_Instructors_Name");
            });

            modelBuilder.Entity<LocationEntity>(entity =>
            {
                entity.ToTable("Locations", tb =>
                    tb.HasCheckConstraint("CK_Locations_PostalCode_NotEmpty", "LTRIM(RTRIM([PostalCode])) <> ''"));

                entity.HasKey(e => e.Id).HasName("PK_Locations_Id");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.StreetName)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.HasIndex(e => e.PostalCode)
                    .HasDatabaseName("IX_Locations_PostalCode");
            });

            modelBuilder.Entity<ParticipantEntity>(entity =>
            {
                entity.ToTable("Participants", tb =>
                    tb.HasCheckConstraint("CK_Participants_Email_NotEmpty", "LTRIM(RTRIM([Email])) <> ''"));

                entity.HasKey(e => e.Id).HasName("PK_Participants_Id");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("(NEWSEQUENTIALID())", "DF_Participants_Id");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.Concurrency)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .IsRequired();

                entity.Property(e => e.CreatedAtUtc)
                    .HasPrecision(0)
                    .HasDefaultValueSql("(SYSUTCDATETIME())", "DF_Participants_CreatedAtUtc")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.ModifiedAtUtc)
                    .HasPrecision(0)
                    .HasDefaultValueSql("(SYSUTCDATETIME())", "DF_Participants_ModifiedAtUtc")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Participants_Email");
            });

            // ========================================
            // DEPENDENT ENTITIES (Have foreign keys)
            // ========================================

            modelBuilder.Entity<InPlaceLocationEntity>(entity =>
            {
                entity.ToTable("InPlaceLocations");

                entity.HasKey(e => e.Id).HasName("PK_InPlaceLocations_Id");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.RoomNumber)
                    .IsRequired();

                entity.Property(e => e.Seats)
                    .IsRequired();

                entity.HasOne(ipl => ipl.Location)
                    .WithMany(l => l.InPlaceLocations)
                    .HasForeignKey(ipl => ipl.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.LocationId, e.RoomNumber })
                    .IsUnique()
                    .HasDatabaseName("IX_InPlaceLocations_LocationId_RoomNumber");
            });

            modelBuilder.Entity<CourseEventEntity>(entity =>
            {
                entity.ToTable("CourseEvents", tb =>
                {
                    tb.HasCheckConstraint("CK_CourseEvents_Price", "[Price] >= 0");
                    tb.HasCheckConstraint("CK_CourseEvents_Seats", "[Seats] > 0");
                });

                entity.HasKey(e => e.Id).HasName("PK_CourseEvents_Id");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("(NEWSEQUENTIALID())", "DF_CourseEvents_Id");

                entity.Property(e => e.EventDate)
                    .HasPrecision(0)
                    .IsRequired();

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .IsRequired();

                entity.Property(e => e.Seats)
                    .IsRequired();

                entity.Property(e => e.Concurrency)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .IsRequired();

                entity.Property(e => e.CreatedAtUtc)
                    .HasPrecision(0)
                    .HasDefaultValueSql("(SYSUTCDATETIME())", "DF_CourseEvents_CreatedAtUtc")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.ModifiedAtUtc)
                    .HasPrecision(0)
                    .HasDefaultValueSql("(SYSUTCDATETIME())", "DF_CourseEvents_ModifiedAtUtc")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasIndex(e => new { e.CourseId, e.EventDate })
                    .HasDatabaseName("IX_CourseEvents_CourseId_EventDate");

                entity.HasOne(ce => ce.CourseEventType)
                    .WithMany(cet => cet.CourseEvents)
                    .HasForeignKey(ce => ce.CourseEventTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CourseRegistrationEntity>(entity =>
            {
                entity.ToTable("CourseRegistrations");

                entity.HasKey(e => e.Id).HasName("PK_CourseRegistrations_Id");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("(NEWSEQUENTIALID())", "DF_CourseRegistrations_Id");

                entity.Property(e => e.RegistrationDate)
                    .HasPrecision(0)
                    .HasDefaultValueSql("(SYSUTCDATETIME())", "DF_CourseRegistrations_RegistrationDate")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.IsPaid)
                    .HasDefaultValue(false)
                    .IsRequired();

                entity.Property(e => e.Concurrency)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .IsRequired();

                entity.Property(e => e.ModifiedAtUtc)
                    .HasPrecision(0)
                    .HasDefaultValueSql("(SYSUTCDATETIME())", "DF_CourseRegistrations_ModifiedAtUtc")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasIndex(e => new { e.ParticipantId, e.CourseEventId })
                    .IsUnique()
                    .HasDatabaseName("IX_CourseRegistrations_ParticipantId_CourseEventId");

                entity.HasOne(cr => cr.Participant)
                    .WithMany(p => p.CourseRegistrations)
                    .HasForeignKey(cr => cr.ParticipantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(cr => cr.CourseEvent)
                    .WithMany(ce => ce.Registrations)
                    .HasForeignKey(cr => cr.CourseEventId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ========================================
            // MANY-TO-MANY RELATIONSHIPS
            // ========================================

            modelBuilder.Entity<CourseEventEntity>()
                .HasMany(ce => ce.Instructors)
                .WithMany(i => i.CourseEvents)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseEventInstructors",
                    i => i.HasOne<InstructorEntity>().WithMany().HasForeignKey("InstructorId").OnDelete(DeleteBehavior.Cascade),
                    ce => ce.HasOne<CourseEventEntity>().WithMany().HasForeignKey("CourseEventId").OnDelete(DeleteBehavior.Cascade),
                    e =>
                    {
                        e.HasKey("CourseEventId", "InstructorId");
                        e.ToTable("CourseEventInstructors");
                    }
                );

            modelBuilder.Entity<CourseEventEntity>()
                .HasMany(ce => ce.InPlaceLocations)
                .WithMany(ipl => ipl.CourseEvents)
                .UsingEntity<Dictionary<string, object>>(
                    "InPlaceEventLocations",
                    ipl => ipl.HasOne<InPlaceLocationEntity>().WithMany().HasForeignKey("InPlaceLocationId").OnDelete(DeleteBehavior.Cascade),
                    ce => ce.HasOne<CourseEventEntity>().WithMany().HasForeignKey("CourseEventId").OnDelete(DeleteBehavior.Cascade),
                    e =>
                    {
                        e.HasKey("CourseEventId", "InPlaceLocationId");
                        e.ToTable("InPlaceEventLocations");
                    }
                );
        }
    }
}
