using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Context;

public sealed class CoursesOnlineDbContext(DbContextOptions<CoursesOnlineDbContext> options) : DbContext(options)
{
    public DbSet<CourseEntity> Courses => Set<CourseEntity>();
    public DbSet<CourseEventEntity> CourseEvents => Set<CourseEventEntity>();
    public DbSet<CourseEventTypeEntity> CourseEventTypes => Set<CourseEventTypeEntity>();
    public DbSet<InstructorEntity> Instructors => Set<InstructorEntity>();
    public DbSet<InstructorRoleEntity> InstructorRoles => Set<InstructorRoleEntity>();
    public DbSet<LocationEntity> Locations => Set<LocationEntity>();
    public DbSet<ParticipantEntity> Participants => Set<ParticipantEntity>();
    public DbSet<InPlaceLocationEntity> InPlaceLocations => Set<InPlaceLocationEntity>();
    public DbSet<CourseRegistrationEntity> CourseRegistrations => Set<CourseRegistrationEntity>();
    public DbSet<CourseRegistrationStatusEntity> CourseRegistrationStatuses => Set<CourseRegistrationStatusEntity>();
    public DbSet<PaymentMethodEntity> PaymentMethods => Set<PaymentMethodEntity>();
    public DbSet<ParticipantContactTypeEntity> ParticipantContactTypes => Set<ParticipantContactTypeEntity>();
    public DbSet<VenueTypeEntity> VenueTypes => Set<VenueTypeEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoursesOnlineDbContext).Assembly);
    }
}

