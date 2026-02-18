using Backend.Domain.Modules.VenueTypes.Models;

namespace Backend.Application.Modules.CourseEvents.Inputs;

public sealed record UpdateCourseEventInput(
    Guid Id,
    Guid CourseId,
    DateTime EventDate,
    decimal Price,
    int Seats,
    int CourseEventTypeId,
    VenueType VenueType
);
