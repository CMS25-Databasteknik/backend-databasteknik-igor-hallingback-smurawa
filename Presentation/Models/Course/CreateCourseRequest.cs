namespace Backend.Presentation.API.Models.Course;

public sealed record CreateCourseRequest
(
    string Title,
    string Description,
    int DurationInDays
);