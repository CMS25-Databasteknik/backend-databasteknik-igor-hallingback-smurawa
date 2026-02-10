using Backend.Domain.Models.CourseEvent;

namespace Backend.Domain.Models.Course
{
    public sealed record CourseDto(
        Guid Id,
        string Title,
        string Description,
        int DurationInDays,
        IEnumerable<CourseEventDto> CourseEvents
    );
}
