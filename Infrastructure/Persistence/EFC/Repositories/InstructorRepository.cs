using Backend.Domain.Modules.Instructors.Contracts;
using Backend.Domain.Modules.Instructors.Models;
using Backend.Infrastructure.Persistence.Entities;
using Backend.Infrastructure.Persistence.EFC.Context;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Repositories;

public class InstructorRepository(CoursesOnlineDbContext context) : IInstructorRepository
{
    private readonly CoursesOnlineDbContext _context = context;

    private static Instructor ToModel(InstructorEntity entity)
        => new(entity.Id, entity.Name);

    public async Task<Instructor> CreateInstructorAsync(Instructor instructor, CancellationToken cancellationToken)
    {
        var entity = new InstructorEntity
        {
            Id = instructor.Id,
            Name = instructor.Name
        };

        _context.Instructors.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return ToModel(entity);
    }

    public async Task<bool> DeleteInstructorAsync(Guid instructorId, CancellationToken cancellationToken)
    {
        var entity = await _context.Instructors.SingleOrDefaultAsync(i => i.Id == instructorId, cancellationToken);

        if (entity == null)
            throw new KeyNotFoundException($"Instructor '{instructorId}' not found.");

        _context.Instructors.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<Instructor>> GetAllInstructorsAsync(CancellationToken cancellationToken)
    {
        var entities = await _context.Instructors
            .AsNoTracking()
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);

        return [.. entities.Select(ToModel)];
    }

    public async Task<Instructor?> GetInstructorByIdAsync(Guid instructorId, CancellationToken cancellationToken)
    {
        var entity = await _context.Instructors
            .AsNoTracking()
            .SingleOrDefaultAsync(i => i.Id == instructorId, cancellationToken);

        return entity == null ? null : ToModel(entity);
    }

    public async Task<Instructor?> UpdateInstructorAsync(Instructor instructor, CancellationToken cancellationToken)
    {
        var entity = await _context.Instructors.SingleOrDefaultAsync(i => i.Id == instructor.Id, cancellationToken);

        if (entity == null)
            throw new KeyNotFoundException($"Instructor '{instructor.Id}' not found.");

        entity.Name = instructor.Name;

        await _context.SaveChangesAsync(cancellationToken);

        return ToModel(entity);
    }

    public async Task<bool> HasCourseEventsAsync(Guid instructorId, CancellationToken cancellationToken)
    {
        return await _context.Instructors
            .AsNoTracking()
            .Where(i => i.Id == instructorId)
            .SelectMany(i => i.CourseEvents)
            .AnyAsync(cancellationToken);
    }
}




