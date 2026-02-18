using Backend.Application.Common;
using Backend.Domain.Modules.CourseRegistrationStatuses.Models;

namespace Backend.Application.Modules.CourseRegistrationStatuses.Outputs;

public sealed class CourseRegistrationStatusResult : ResultCommon<CourseRegistrationStatus>
{
}

public sealed class CourseRegistrationStatusListResult : ResultCommon<IEnumerable<CourseRegistrationStatus>>
{
}

public sealed class CourseRegistrationStatusDeleteResult : ResultCommon<bool>
{
}