namespace Backend.Domain.Models.Course
{
    public sealed record CreateCourseDto(string Title, string Description, int DurationInDays);
}
