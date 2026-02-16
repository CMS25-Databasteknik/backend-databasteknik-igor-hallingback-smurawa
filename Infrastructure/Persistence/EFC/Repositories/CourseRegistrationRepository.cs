using Backend.Domain.Modules.CourseRegistrations.Contracts;
using Backend.Domain.Modules.CourseRegistrations.Models;
using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Repositories;

public class CourseRegistrationRepository(CoursesOnlineDbContext context) : ICourseRegistrationRepository
{
    private readonly CoursesOnlineDbContext _context = context;

    private static CourseRegistration ToModel(CourseRegistrationEntity entity)
        => new(entity.Id, entity.ParticipantId, entity.CourseEventId, entity.RegistrationDate, entity.IsPaid);

    public async Task<CourseRegistration> CreateCourseRegistrationAsync(CourseRegistration courseRegistration, CancellationToken cancellationToken)
    {
        var entity = new CourseRegistrationEntity
        {
            Id = courseRegistration.Id,
            ParticipantId = courseRegistration.ParticipantId,
            CourseEventId = courseRegistration.CourseEventId,
            IsPaid = courseRegistration.IsPaid
        };

        _context.CourseRegistrations.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return ToModel(entity);
    }

    public async Task<bool> DeleteCourseRegistrationAsync(Guid courseRegistrationId, CancellationToken cancellationToken)
    {
        var entity = await _context.CourseRegistrations.SingleOrDefaultAsync(cr => cr.Id == courseRegistrationId, cancellationToken);

        if (entity == null)
            throw new KeyNotFoundException($"Course registration '{courseRegistrationId}' not found.");

        _context.CourseRegistrations.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<CourseRegistration>> GetAllCourseRegistrationsAsync(CancellationToken cancellationToken)
    {
        var entities = await _context.CourseRegistrations
            .AsNoTracking()
            .OrderByDescending(cr => cr.RegistrationDate)
            .ToListAsync(cancellationToken);

        return [.. entities.Select(ToModel)];
    }

    public async Task<CourseRegistration?> GetCourseRegistrationByIdAsync(Guid courseRegistrationId, CancellationToken cancellationToken)
    {
        var entity = await _context.CourseRegistrations
            .AsNoTracking()
            .SingleOrDefaultAsync(cr => cr.Id == courseRegistrationId, cancellationToken);

        return entity == null ? null : ToModel(entity);
    }

    public async Task<IReadOnlyList<CourseRegistration>> GetCourseRegistrationsByParticipantIdAsync(Guid participantId, CancellationToken cancellationToken)
    {
        var entities = await _context.CourseRegistrations
            .AsNoTracking()
            .Where(cr => cr.ParticipantId == participantId)
            .OrderByDescending(cr => cr.RegistrationDate)
            .ToListAsync(cancellationToken);

        return [.. entities.Select(ToModel)];
    }

    public async Task<IReadOnlyList<CourseRegistration>> GetCourseRegistrationsByCourseEventIdAsync(Guid courseEventId, CancellationToken cancellationToken)
    {
        var entities = await _context.CourseRegistrations
            .AsNoTracking()
            .Where(cr => cr.CourseEventId == courseEventId)
            .OrderByDescending(cr => cr.RegistrationDate)
            .ToListAsync(cancellationToken);

        return [.. entities.Select(ToModel)];
    }

    public async Task<CourseRegistration?> UpdateCourseRegistrationAsync(CourseRegistration courseRegistration, CancellationToken cancellationToken)
    {
        var entity = await _context.CourseRegistrations.SingleOrDefaultAsync(cr => cr.Id == courseRegistration.Id, cancellationToken);

        if (entity == null)
            throw new KeyNotFoundException($"Course registration '{courseRegistration.Id}' not found.");

        entity.ParticipantId = courseRegistration.ParticipantId;
        entity.CourseEventId = courseRegistration.CourseEventId;
        entity.IsPaid = courseRegistration.IsPaid;
        entity.ModifiedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ToModel(entity);
    }
}
