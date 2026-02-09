namespace Backend.Domain.Models.Course
{
    public sealed record CourseDto(Guid Id, string Title, string Description, int DurationInDays);
}
