namespace Backend.Domain.Models.CourseEvent
{
    public sealed record CourseEventDto(
        Guid Id,
        DateTime EventDate,
        decimal Price,
        int Seats,
        int CourseEventTypeId
    );
}
