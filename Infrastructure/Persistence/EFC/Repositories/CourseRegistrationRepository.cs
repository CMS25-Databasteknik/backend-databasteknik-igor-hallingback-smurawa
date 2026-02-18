using Backend.Domain.Modules.CourseRegistrations.Contracts;
using Backend.Domain.Modules.CourseRegistrations.Models;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Repositories;

public class CourseRegistrationRepository(CoursesOnlineDbContext context) : ICourseRegistrationRepository
{
    private readonly CoursesOnlineDbContext _context = context;

    private static CourseRegistrationStatus ToStatusModel(CourseRegistrationEntity entity)
    {
        var statusName = entity.CourseRegistrationStatus?.Name;

        if (!string.IsNullOrWhiteSpace(statusName))
            return new CourseRegistrationStatus(entity.CourseRegistrationStatusId, statusName);

        return entity.CourseRegistrationStatusId switch
        {
            0 => CourseRegistrationStatus.Pending,
            1 => CourseRegistrationStatus.Paid,
            2 => CourseRegistrationStatus.Cancelled,
            3 => CourseRegistrationStatus.Refunded,
            _ => new CourseRegistrationStatus(entity.CourseRegistrationStatusId, $"Status {entity.CourseRegistrationStatusId}")
        };
    }

    private static CourseRegistration ToModel(CourseRegistrationEntity entity)
        => new(entity.Id, entity.ParticipantId, entity.CourseEventId, entity.RegistrationDate, ToStatusModel(entity), (PaymentMethod)entity.PaymentMethodId);

    public async Task<CourseRegistration> CreateCourseRegistrationAsync(CourseRegistration courseRegistration, CancellationToken cancellationToken)
    {
        using var tx = await _context.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable,
            cancellationToken);

        try
        {
            var availableSeats = await _context.Database
                .SqlQuery<int>(
                    $"""
                    SELECT ce.Seats - COALESCE(COUNT(cr.Id), 0) AS Value
                    FROM CourseEvents ce
                    LEFT JOIN CourseRegistrations cr ON ce.Id = cr.CourseEventId
                    WHERE ce.Id = {courseRegistration.CourseEventId}
                    GROUP BY ce.Id, ce.Seats
                    """)
                .FirstOrDefaultAsync(cancellationToken);

            if (availableSeats <= 0)
                throw new InvalidOperationException($"No available seats for course event '{courseRegistration.CourseEventId}'.");

            var entity = new CourseRegistrationEntity
            {
                Id = courseRegistration.Id,
                ParticipantId = courseRegistration.ParticipantId,
                CourseEventId = courseRegistration.CourseEventId,
                CourseRegistrationStatusId = courseRegistration.Status.Id,
                PaymentMethodId = (int)courseRegistration.PaymentMethod
            };

            _context.CourseRegistrations.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return ToModel(entity);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
    public async Task<CourseRegistration?> CreateRegistrationWithSeatCheckAsync(
        CourseRegistration courseRegistration,
        CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable,
            cancellationToken);

        try
        {
            var availableSeats = await _context.Database
                .SqlQuery<int>(
                    $"""
                    SELECT ce.Seats - COALESCE(COUNT(cr.Id), 0) AS Value
                    FROM CourseEvents ce
                    LEFT JOIN CourseRegistrations cr ON ce.Id = cr.CourseEventId
                    WHERE ce.Id = {courseRegistration.CourseEventId}
                    GROUP BY ce.Id, ce.Seats
                    """)
                .FirstOrDefaultAsync(cancellationToken);

            if (availableSeats <= 0)
            {
                await transaction.RollbackAsync(cancellationToken);
                return null;
            }

            var entity = new CourseRegistrationEntity
            {
                Id = courseRegistration.Id,
                ParticipantId = courseRegistration.ParticipantId,
                CourseEventId = courseRegistration.CourseEventId,
                CourseRegistrationStatusId = courseRegistration.Status.Id,
                PaymentMethodId = (int)courseRegistration.PaymentMethod
            };

            _context.CourseRegistrations.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return ToModel(entity);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
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
            .Include(cr => cr.CourseRegistrationStatus)
            .OrderByDescending(cr => cr.RegistrationDate)
            .ToListAsync(cancellationToken);

        return [.. entities.Select(ToModel)];
    }

    public async Task<CourseRegistration?> GetCourseRegistrationByIdAsync(Guid courseRegistrationId, CancellationToken cancellationToken)
    {
        var entity = await _context.CourseRegistrations
            .AsNoTracking()
            .Include(cr => cr.CourseRegistrationStatus)
            .SingleOrDefaultAsync(cr => cr.Id == courseRegistrationId, cancellationToken);

        return entity == null ? null : ToModel(entity);
    }

    public async Task<IReadOnlyList<CourseRegistration>> GetCourseRegistrationsByParticipantIdAsync(Guid participantId, CancellationToken cancellationToken)
    {
        var entities = await _context.CourseRegistrations
            .AsNoTracking()
            .Include(cr => cr.CourseRegistrationStatus)
            .Where(cr => cr.ParticipantId == participantId)
            .OrderByDescending(cr => cr.RegistrationDate)
            .ToListAsync(cancellationToken);

        return [.. entities.Select(ToModel)];
    }

    public async Task<IReadOnlyList<CourseRegistration>> GetCourseRegistrationsByCourseEventIdAsync(Guid courseEventId, CancellationToken cancellationToken)
    {
        var entities = await _context.CourseRegistrations
            .AsNoTracking()
            .Include(cr => cr.CourseRegistrationStatus)
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
        entity.CourseRegistrationStatusId = courseRegistration.Status.Id;
        entity.PaymentMethodId = (int)courseRegistration.PaymentMethod;
        entity.ModifiedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ToModel(entity);
    }
}





