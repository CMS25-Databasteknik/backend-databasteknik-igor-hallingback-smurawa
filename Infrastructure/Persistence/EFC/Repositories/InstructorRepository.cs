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
    {
        if (entity.InstructorRole == null)
            throw new InvalidOperationException("InstructorRole must be included when mapping Instructor.");

        var role = new InstructorRole(entity.InstructorRole.Id, entity.InstructorRole.RoleName);
        return new Instructor(entity.Id, entity.Name, role);
    }

    public async Task<Instructor> CreateInstructorAsync(Instructor instructor, CancellationToken cancellationToken)
    {
        var roleExists = await _context.InstructorRoles
            .AsNoTracking()
            .SingleOrDefaultAsync(r => r.Id == instructor.InstructorRoleId, cancellationToken);
        if (roleExists == null)
            throw new KeyNotFoundException($"Instructor role '{instructor.InstructorRoleId}' not found.");

        var entity = new InstructorEntity
        {
            Id = instructor.Id,
            Name = instructor.Name,
            InstructorRoleId = instructor.InstructorRoleId
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
            .Include(i => i.InstructorRole)
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);

        return [.. entities.Select(ToModel)];
    }

    public async Task<Instructor?> GetInstructorByIdAsync(Guid instructorId, CancellationToken cancellationToken)
    {
        var entity = await _context.Instructors
            .AsNoTracking()
            .Include(i => i.InstructorRole)
            .SingleOrDefaultAsync(i => i.Id == instructorId, cancellationToken);

        return entity == null ? null : ToModel(entity);
    }

    public async Task<Instructor?> UpdateInstructorAsync(Instructor instructor, CancellationToken cancellationToken)
    {
        var roleEntity = await _context.InstructorRoles
            .AsNoTracking()
            .SingleOrDefaultAsync(r => r.Id == instructor.InstructorRoleId, cancellationToken);
        if (roleEntity == null)
            throw new KeyNotFoundException($"Instructor role '{instructor.InstructorRoleId}' not found.");

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





