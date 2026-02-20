using Backend.Application.Common;
using Backend.Domain.Modules.CourseEvents.Models;

namespace Backend.Application.Modules.CourseEvents.Outputs;

public sealed class CourseEventResult : ResultCommon<CourseEvent>
{
}

public sealed class CourseEventListResult : ResultCommon<IReadOnlyList<CourseEvent>>
{
}

public sealed class CourseEventDeleteResult : ResultCommon<bool>
{
}

