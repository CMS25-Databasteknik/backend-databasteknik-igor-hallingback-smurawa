namespace Backend.Application.Modules.Courses.Inputs;

public sealed record CreateCourseEventInput(
    Guid CourseId,
    DateTime EventDate,
    decimal Price,
    int Seats,
    int CourseEventTypeId
);
