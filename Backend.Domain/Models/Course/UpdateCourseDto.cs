namespace Backend.Domain.Models.Course
{
    public sealed record UpdateCourseDto(Guid Id, string Title, string Description, int DurationInDays);
}
