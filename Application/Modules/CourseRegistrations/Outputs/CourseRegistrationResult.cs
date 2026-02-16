using Backend.Application.Common;
using Backend.Domain.Modules.CourseRegistrations.Models;

namespace Backend.Application.Modules.CourseRegistrations.Outputs;

public sealed class CourseRegistrationResult : ResultCommon<CourseRegistration>
{
}

public sealed class CourseRegistrationListResult : ResultCommon<IEnumerable<CourseRegistration>>
{
}

public sealed class CourseRegistrationDeleteResult : ResultCommon<bool>
{
}
