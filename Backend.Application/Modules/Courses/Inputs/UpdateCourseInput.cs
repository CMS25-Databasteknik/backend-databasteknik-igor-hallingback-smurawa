namespace Backend.Application.Modules.Courses.Inputs;

public sealed record UpdateCourseInput
(
    string Title,
    string Description,
    int DurationInDays
);
