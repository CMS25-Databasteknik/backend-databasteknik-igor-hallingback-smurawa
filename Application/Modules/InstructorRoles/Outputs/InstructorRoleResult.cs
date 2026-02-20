using Backend.Application.Common;
using Backend.Domain.Modules.InstructorRoles.Models;

namespace Backend.Application.Modules.InstructorRoles.Outputs;

public sealed class InstructorRoleResult : ResultCommon<InstructorRole>
{
}

public sealed class InstructorRoleListResult : ResultCommon<IReadOnlyList<InstructorRole>>
{
}

public sealed class InstructorRoleDeleteResult : ResultCommon<bool>
{
}
