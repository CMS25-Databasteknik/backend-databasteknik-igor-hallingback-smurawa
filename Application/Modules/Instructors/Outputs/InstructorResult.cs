using Backend.Application.Common;
using Backend.Domain.Modules.Instructors.Models;

namespace Backend.Application.Modules.Instructors.Outputs;

public sealed record InstructorLookupItem(int Id, string Name);

public sealed record InstructorDetails(
    Guid Id,
    string Name,
    InstructorLookupItem InstructorRole
);

public sealed class InstructorResult : ResultCommon<Instructor>
{
}

public sealed class InstructorDetailsResult : ResultCommon<InstructorDetails>
{
}

public sealed class InstructorListResult : ResultCommon<IReadOnlyList<Instructor>>
{
}

public sealed class InstructorDeleteResult : ResultCommon<bool>
{
}

