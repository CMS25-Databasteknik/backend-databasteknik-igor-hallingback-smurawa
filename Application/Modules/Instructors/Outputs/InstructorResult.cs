using Backend.Application.Common;
using Backend.Domain.Modules.Instructors.Models;

namespace Backend.Application.Modules.Instructors.Outputs;

public sealed record InstructorLookupItem(int Id, string Name);

public sealed record InstructorDetails(
    Guid Id,
    string Name,
    InstructorLookupItem InstructorRole
);

public sealed record InstructorResult : ResultBase<Instructor>
{
}

public sealed record InstructorDetailsResult : ResultBase<InstructorDetails>
{
}

public sealed record InstructorListResult : ResultBase<IReadOnlyList<Instructor>>
{
}

public sealed record InstructorDeleteResult : ResultBase<bool>
{
}

