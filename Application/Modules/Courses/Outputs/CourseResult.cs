using Backend.Application.Common;
using Backend.Domain.Modules.Courses.Models;

namespace Backend.Application.Modules.Courses.Outputs;

public sealed record CourseResult : ResultBase<Course>
{
}

public sealed record CourseWithEventsResult : ResultBase<CourseWithEvents>
{
}

public sealed record CourseListResult : ResultBase<IReadOnlyList<Course>>
{
}

public sealed record CourseDeleteResult : ResultBase<bool>
{
}
