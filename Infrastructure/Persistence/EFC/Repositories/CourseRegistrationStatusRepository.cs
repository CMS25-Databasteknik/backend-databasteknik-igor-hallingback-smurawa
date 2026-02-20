using Backend.Domain.Modules.CourseRegistrationStatuses.Contracts;
using Backend.Domain.Modules.CourseRegistrationStatuses.Models;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Repositories;

public sealed class CourseRegistrationStatusRepository(
    CoursesOnlineDbContext context) : ICourseRegistrationStatusRepository
{
    private readonly CoursesOnlineDbContext _context = context;

    private static CourseRegistrationStatus ToModel(CourseRegistrationStatusEntity entity)
        => new(entity.Id, entity.Name);

    public async Task<CourseRegistrationStatus> CreateCourseRegistrationStatusAsync(CourseRegistrationStatus status, CancellationToken cancellationToken)
    {
        var currentMaxId = await _context.CourseRegistrationStatuses
            .AsNoTracking()
            .MaxAsync(s => (int?)s.Id, cancellationToken);

        var entity = new CourseRegistrationStatusEntity
        {
            Id = (currentMaxId ?? -1) + 1,
            Name = status.Name
        };

        _context.CourseRegistrationStatuses.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return ToModel(entity);
    }

    public async Task<IReadOnlyList<CourseRegistrationStatus>> GetAllCourseRegistrationStatusesAsync(CancellationToken cancellationToken)
    {
        var entities = await _context.CourseRegistrationStatuses
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .ToListAsync(cancellationToken);

        return [.. entities.Select(ToModel)];
    }

    public async Task<CourseRegistrationStatus?> GetCourseRegistrationStatusByIdAsync(int statusId, CancellationToken cancellationToken)
    {
        if (statusId < 0)
            throw new ArgumentException("Status ID must be zero or positive.", nameof(statusId));

        var entity = await _context.CourseRegistrationStatuses
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id == statusId, cancellationToken);

        return entity == null ? null : ToModel(entity);
    }

    public async Task<CourseRegistrationStatus?> GetCourseRegistrationStatusByNameAsync(string name, CancellationToken cancellationToken)
    {
        var entity = await _context.CourseRegistrationStatuses
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Name == name, cancellationToken);

        return entity == null ? null : ToModel(entity);
    }

    public async Task<CourseRegistrationStatus?> UpdateCourseRegistrationStatusAsync(CourseRegistrationStatus status, CancellationToken cancellationToken)
    {
        var entity = await _context.CourseRegistrationStatuses
            .SingleOrDefaultAsync(s => s.Id == status.Id, cancellationToken);

        if (entity == null)
            throw new KeyNotFoundException($"Course registration status '{status.Id}' not found.");

        entity.Name = status.Name;

        await _context.SaveChangesAsync(cancellationToken);

        return ToModel(entity);
    }

    public async Task<bool> DeleteCourseRegistrationStatusAsync(int statusId, CancellationToken cancellationToken)
    {
        var entity = await _context.CourseRegistrationStatuses
            .SingleOrDefaultAsync(s => s.Id == statusId, cancellationToken);

        if (entity == null)
            throw new KeyNotFoundException($"Course registration status '{statusId}' not found.");

        _context.CourseRegistrationStatuses.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> IsInUseAsync(int statusId, CancellationToken cancellationToken)
    {
        if (statusId < 0)
            throw new ArgumentException("Status ID must be zero or positive.", nameof(statusId));

        return await _context.CourseRegistrations
            .AsNoTracking()
            .AnyAsync(cr => cr.CourseRegistrationStatusId == statusId, cancellationToken);
    }

}
