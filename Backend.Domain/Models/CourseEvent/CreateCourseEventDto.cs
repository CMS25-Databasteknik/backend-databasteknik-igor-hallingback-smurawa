namespace Backend.Domain.Models.CourseEvent
{
    public sealed record CreateCourseEventDto(
        Guid CourseId,
        DateTime EventDate,
        decimal Price,
        int Seats,
        int CourseEventTypeId
    );
}
