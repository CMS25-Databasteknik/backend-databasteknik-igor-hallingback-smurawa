using Backend.Domain.Modules.InstructorRoles.Models;

namespace Backend.Domain.Modules.InstructorRoles.Contracts;

public interface IInstructorRoleRepository
{
    Task<InstructorRole> CreateInstructorRoleAsync(InstructorRole role, CancellationToken cancellationToken);
    Task<IReadOnlyList<InstructorRole>> GetAllInstructorRolesAsync(CancellationToken cancellationToken);
    Task<InstructorRole?> GetInstructorRoleByIdAsync(int id, CancellationToken cancellationToken);
    Task<InstructorRole?> UpdateInstructorRoleAsync(InstructorRole role, CancellationToken cancellationToken);
    Task<bool> DeleteInstructorRoleAsync(int id, CancellationToken cancellationToken);
}
