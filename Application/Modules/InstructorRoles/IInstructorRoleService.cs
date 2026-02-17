using Backend.Application.Modules.InstructorRoles.Inputs;
using Backend.Application.Modules.InstructorRoles.Outputs;

namespace Backend.Application.Modules.InstructorRoles;

public interface IInstructorRoleService
{
    Task<InstructorRoleResult> CreateInstructorRoleAsync(CreateInstructorRoleInput input, CancellationToken cancellationToken = default);
    Task<InstructorRoleListResult> GetAllInstructorRolesAsync(CancellationToken cancellationToken = default);
    Task<InstructorRoleResult> GetInstructorRoleByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<InstructorRoleResult> UpdateInstructorRoleAsync(UpdateInstructorRoleInput input, CancellationToken cancellationToken = default);
    Task<InstructorRoleDeleteResult> DeleteInstructorRoleAsync(int id, CancellationToken cancellationToken = default);
}
