namespace Backend.Presentation.API.Models.Course;

public sealed record UpdateCourseRequest
(
    string Title,
    string Description,
    int DurationInDays
);
