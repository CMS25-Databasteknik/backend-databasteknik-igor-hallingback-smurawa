using Backend.Application.Common;
using Backend.Domain.Modules.CourseEventTypes.Models;

namespace Backend.Application.Modules.CourseEventTypes.Outputs;

public sealed class CourseEventTypeResult : ResultCommon<CourseEventType>
{
}

public sealed class CourseEventTypeListResult : ResultCommon<IEnumerable<CourseEventType>>
{
}

public sealed class CourseEventTypeDeleteResult : ResultCommon<bool>
{
}
