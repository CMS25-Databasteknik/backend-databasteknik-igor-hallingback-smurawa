using Backend.Application.Common;
using Backend.Domain.Modules.InstructorRoles.Models;

namespace Backend.Application.Modules.InstructorRoles.Outputs;

public sealed record InstructorRoleResult : ResultBase<InstructorRole>
{
}

public sealed record InstructorRoleListResult : ResultBase<IReadOnlyList<InstructorRole>>
{
}

public sealed record InstructorRoleDeleteResult : ResultBase<bool>
{
}
