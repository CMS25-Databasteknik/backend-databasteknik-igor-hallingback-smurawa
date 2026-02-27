using Backend.Application.Common;
using Backend.Domain.Modules.CourseRegistrationStatuses.Models;

namespace Backend.Application.Modules.CourseRegistrationStatuses.Outputs;

public sealed record CourseRegistrationStatusResult : ResultBase<CourseRegistrationStatus>
{
}

public sealed record CourseRegistrationStatusListResult : ResultBase<IReadOnlyList<CourseRegistrationStatus>>
{
}

public sealed record CourseRegistrationStatusDeleteResult : ResultBase<bool>
{
}
