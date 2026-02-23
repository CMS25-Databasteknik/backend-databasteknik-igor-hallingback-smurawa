using Backend.Domain.Modules.CourseRegistrations.Contracts;
using Backend.Domain.Modules.CourseRegistrations.Models;
using Backend.Domain.Modules.CourseRegistrationStatuses.Models;
using Backend.Domain.Modules.PaymentMethod.Models;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Repositories;

public class CourseRegistrationRepository(CoursesOnlineDbContext context)
    : RepositoryBase<CourseRegistration, Guid, CourseRegistrationEntity, CoursesOnlineDbContext>(context), ICourseRegistrationRepository
{
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

    protected override CourseRegistration ToModel(CourseRegistrationEntity entity)
        => new(
            entity.Id,
            entity.ParticipantId,
            entity.CourseEventId,
            entity.RegistrationDate,
            ToStatusModel(entity),
            (PaymentMethod)entity.PaymentMethodId);

    protected override CourseRegistrationEntity ToEntity(CourseRegistration courseRegistration)
        => new()
        {
            Id = courseRegistration.Id,
            ParticipantId = courseRegistration.ParticipantId,
            CourseEventId = courseRegistration.CourseEventId,
            CourseRegistrationStatusId = courseRegistration.Status.Id,
            PaymentMethodId = (int)courseRegistration.PaymentMethod
        };

    public override async Task<CourseRegistration> AddAsync(CourseRegistration courseRegistration, CancellationToken cancellationToken)
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

            var entity = ToEntity(courseRegistration);

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

            var entity = ToEntity(courseRegistration);

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

    public override async Task<bool> RemoveAsync(Guid courseRegistrationId, CancellationToken cancellationToken)
    {
        var entity = await _context.CourseRegistrations.SingleOrDefaultAsync(cr => cr.Id == courseRegistrationId, cancellationToken);
        if (entity == null)
            throw new KeyNotFoundException($"Course registration '{courseRegistrationId}' not found.");

        _context.CourseRegistrations.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public override async Task<IReadOnlyList<CourseRegistration>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await _context.CourseRegistrations
            .AsNoTracking()
            .OrderByDescending(cr => cr.RegistrationDate)
            .ToListAsync(cancellationToken);

        return [.. entities.Select(ToModel)];
    }

    public override async Task<CourseRegistration?> GetByIdAsync(Guid courseRegistrationId, CancellationToken cancellationToken)
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

    public override async Task<CourseRegistration?> UpdateAsync(Guid id, CourseRegistration courseRegistration, CancellationToken cancellationToken)
    {
        var entity = await _context.CourseRegistrations.SingleOrDefaultAsync(cr => cr.Id == id, cancellationToken);

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







