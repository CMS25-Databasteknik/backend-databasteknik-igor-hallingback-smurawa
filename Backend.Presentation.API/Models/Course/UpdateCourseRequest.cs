namespace Backend.Presentation.API.Models.Course;

public sealed record UpdateCourseRequest
(
    Guid Id,
    string Title,
    string Description,
    int DurationInDays
);
