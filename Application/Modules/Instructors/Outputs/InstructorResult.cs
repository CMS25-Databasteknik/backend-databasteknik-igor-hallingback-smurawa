using Backend.Application.Common;
using Backend.Domain.Modules.Instructors.Models;

namespace Backend.Application.Modules.Instructors.Outputs;

public sealed class InstructorResult : ResultCommon<Instructor>
{
}

public sealed class InstructorListResult : ResultCommon<IEnumerable<Instructor>>
{
}

public sealed class InstructorDeleteResult : ResultCommon<bool>
{
}

