using Backend.Domain.Modules.CourseEventTypes.Contracts;
using Backend.Domain.Modules.CourseEventTypes.Models;
using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Repositories;

public class CourseEventTypeRepository(CoursesOnlineDbContext context) : ICourseEventTypesRepository
{
    private readonly CoursesOnlineDbContext _context = context;

    private static CourseEventType ToModel(CourseEventTypeEntity entity)
        => new(entity.Id, entity.TypeName);

    public async Task<CourseEventType> CreateCourseEventTypeAsync(CourseEventType courseEventType, CancellationToken cancellationToken)
    {
        var entity = new CourseEventTypeEntity
        {
            TypeName = courseEventType.TypeName
        };

        _context.CourseEventTypes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return ToModel(entity);
    }

    public async Task<bool> DeleteCourseEventTypeAsync(int courseEventTypeId, CancellationToken cancellationToken)
    {
        var entity = await _context.CourseEventTypes.SingleOrDefaultAsync(cet => cet.Id == courseEventTypeId, cancellationToken);

        if (entity == null)
            throw new KeyNotFoundException($"Course event type '{courseEventTypeId}' not found.");

        _context.CourseEventTypes.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<CourseEventType>> GetAllCourseEventTypesAsync(CancellationToken cancellationToken)
    {
        var entities = await _context.CourseEventTypes
            .AsNoTracking()
            .OrderBy(cet => cet.Id)
            .ToListAsync(cancellationToken);

        return [.. entities.Select(ToModel)];
    }

    public async Task<CourseEventType?> GetCourseEventTypeByIdAsync(int courseEventTypeId, CancellationToken cancellationToken)
    {
        var entity = await _context.CourseEventTypes
            .AsNoTracking()
            .SingleOrDefaultAsync(cet => cet.Id == courseEventTypeId, cancellationToken);

        return entity == null ? null : ToModel(entity);
    }

    public async Task<CourseEventType?> UpdateCourseEventTypeAsync(CourseEventType courseEventType, CancellationToken cancellationToken)
    {
        var entity = await _context.CourseEventTypes.SingleOrDefaultAsync(cet => cet.Id == courseEventType.Id, cancellationToken);

        if (entity == null)
            throw new KeyNotFoundException($"Course event type '{courseEventType.Id}' not found.");

        entity.TypeName = courseEventType.TypeName;

        await _context.SaveChangesAsync(cancellationToken);

        return ToModel(entity);
    }

    public async Task<bool> IsInUseAsync(int courseEventTypeId, CancellationToken cancellationToken)
    {
        return await _context.CourseEvents
            .AsNoTracking()
            .AnyAsync(ce => ce.CourseEventTypeId == courseEventTypeId, cancellationToken);
    }
}
