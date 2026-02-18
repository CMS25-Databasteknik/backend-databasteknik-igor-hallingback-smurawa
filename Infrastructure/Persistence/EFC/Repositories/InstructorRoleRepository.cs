using Backend.Domain.Modules.InstructorRoles.Contracts;
using Backend.Domain.Modules.InstructorRoles.Models;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Repositories;

public class InstructorRoleRepository(CoursesOnlineDbContext context) : IInstructorRoleRepository
{
    private readonly CoursesOnlineDbContext _context = context;

    private static InstructorRole ToModel(InstructorRoleEntity entity) =>
        new(entity.Id, entity.RoleName);

    public async Task<InstructorRole> CreateInstructorRoleAsync(InstructorRole role, CancellationToken cancellationToken)
    {
        var entity = new InstructorRoleEntity
        {
            RoleName = role.RoleName
        };

        _context.InstructorRoles.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new InstructorRole(entity.Id, entity.RoleName);
    }

    public async Task<IReadOnlyList<InstructorRole>> GetAllInstructorRolesAsync(CancellationToken cancellationToken)
    {
        var entities = await _context.InstructorRoles
            .AsNoTracking()
            .OrderBy(r => r.RoleName)
            .ToListAsync(cancellationToken);

        return [.. entities.Select(ToModel)];
    }

    public async Task<InstructorRole?> GetInstructorRoleByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.InstructorRoles
            .AsNoTracking()
            .SingleOrDefaultAsync(r => r.Id == id, cancellationToken);

        return entity == null ? null : ToModel(entity);
    }

    public async Task<InstructorRole?> UpdateInstructorRoleAsync(InstructorRole role, CancellationToken cancellationToken)
    {
        var entity = await _context.InstructorRoles
            .SingleOrDefaultAsync(r => r.Id == role.Id, cancellationToken);

        if (entity == null)
            return null;

        entity.RoleName = role.RoleName;
        await _context.SaveChangesAsync(cancellationToken);

        return ToModel(entity);
    }

    public async Task<bool> DeleteInstructorRoleAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.InstructorRoles.SingleOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (entity == null)
            return false;

        _context.InstructorRoles.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
