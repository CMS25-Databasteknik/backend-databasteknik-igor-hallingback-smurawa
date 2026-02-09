namespace Backend.Domain.Models.CourseEvent
{
    public sealed record UpdateCourseEventDto(
        Guid Id,
        Guid CourseId,
        DateTime EventDate,
        decimal Price,
        int Seats,
        int CourseEventTypeId
    );
}
