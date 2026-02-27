using Backend.Application.Common;
using Backend.Domain.Modules.CourseEvents.Models;

namespace Backend.Application.Modules.CourseEvents.Outputs;

public sealed record CourseEventLookupItem(int Id, string Name);

public sealed record CourseEventDetails(
    Guid Id,
    Guid CourseId,
    DateTime EventDate,
    decimal Price,
    int Seats,
    CourseEventLookupItem CourseEventType,
    CourseEventLookupItem VenueType
);

public sealed record CourseEventResult : ResultBase<CourseEvent>
{
}

public sealed record CourseEventDetailsResult : ResultBase<CourseEventDetails>
{
}

public sealed record CourseEventListResult : ResultBase<IReadOnlyList<CourseEvent>>
{
}

public sealed record CourseEventDeleteResult : ResultBase<bool>
{
}

