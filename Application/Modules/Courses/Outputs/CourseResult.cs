using Backend.Application.Common;
using Backend.Domain.Modules.Courses.Models;
using Backend.Domain.Modules.CourseWithEvents.Models;

namespace Backend.Application.Modules.Courses.Outputs;

public sealed class CourseResult : ResultCommon<Course>
{
}

public sealed class CourseWithEventsResult : ResultCommon<CourseWithEvents>
{
}

public sealed class CourseListResult : ResultCommon<IReadOnlyList<CourseWithEvents>>
{
}

public sealed class CourseDeleteResult : ResultCommon<bool>
{
}
