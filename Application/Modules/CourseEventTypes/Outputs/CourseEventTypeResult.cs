using Backend.Application.Common;
using Backend.Domain.Modules.CourseEventTypes.Models;

namespace Backend.Application.Modules.CourseEventTypes.Outputs;

public sealed record CourseEventTypeResult : ResultBase<CourseEventType>
{
}

public sealed record CourseEventTypeListResult : ResultBase<IReadOnlyList<CourseEventType>>
{
}

public sealed record CourseEventTypeDeleteResult : ResultBase<bool>
{
}
