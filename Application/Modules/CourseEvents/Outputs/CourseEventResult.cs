using Backend.Application.Common;
using Backend.Domain.Modules.CourseEvents.Models;

namespace Backend.Application.Modules.CourseEvents.Outputs;

public sealed record LookupItem(int Id, string Name);

public sealed record CourseEventDetails(
    Guid Id,
    Guid CourseId,
    DateTime EventDate,
    decimal Price,
    int Seats,
    LookupItem CourseEventType,
    LookupItem VenueType
);

public sealed class CourseEventResult : ResultCommon<CourseEvent>
{
}

public sealed class CourseEventDetailsResult : ResultCommon<CourseEventDetails>
{
}

public sealed class CourseEventListResult : ResultCommon<IReadOnlyList<CourseEvent>>
{
}

public sealed class CourseEventDeleteResult : ResultCommon<bool>
{
}

