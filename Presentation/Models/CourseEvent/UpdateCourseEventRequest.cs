namespace Backend.Presentation.API.Models.CourseEvent;

public sealed record UpdateCourseEventRequest
(
    Guid CourseId,
    DateTime EventDate,
    decimal Price,
    int Seats,
    int CourseEventTypeId
);
