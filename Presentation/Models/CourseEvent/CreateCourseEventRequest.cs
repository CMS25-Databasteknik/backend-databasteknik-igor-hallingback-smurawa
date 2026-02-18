using Backend.Domain.Modules.CourseEvents.Models;

namespace Backend.Presentation.API.Models.CourseEvent;

public sealed record CreateCourseEventRequest
(
    Guid CourseId,
    DateTime EventDate,
    decimal Price,
    int Seats,
    int CourseEventTypeId,
    VenueType VenueType
);
