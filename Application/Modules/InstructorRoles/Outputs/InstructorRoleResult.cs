using Backend.Application.Common;
using Backend.Domain.Modules.Instructors.Models;

namespace Backend.Application.Modules.InstructorRoles.Outputs;

public sealed class InstructorRoleResult : ResultCommon<InstructorRole>
{
}

public sealed class InstructorRoleListResult : ResultCommon<IEnumerable<InstructorRole>>
{
}

public sealed class InstructorRoleDeleteResult : ResultCommon<bool>
{
}
