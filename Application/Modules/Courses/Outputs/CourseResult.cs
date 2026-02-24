using Backend.Application.Common;
using Backend.Domain.Modules.Courses.Models;

namespace Backend.Application.Modules.Courses.Outputs;

public sealed class CourseResult : ResultCommon<Course>
{
}

public sealed class CourseWithEventsResult : ResultCommon<CourseWithEvents>
{
}

public sealed class CourseListResult : ResultCommon<IReadOnlyList<Course>>
{
}

public sealed class CourseDeleteResult : ResultCommon<bool>
{
}
