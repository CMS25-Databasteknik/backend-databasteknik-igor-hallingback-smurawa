namespace Backend.Presentation.API.Models.Course;

public sealed class CreateCourseRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required int DurationInDays { get; init; }
}
