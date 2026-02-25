namespace Backend.Presentation.API.Models.CourseEvent;

public sealed class UpdateCourseEventRequest
{
    public required Guid CourseId { get; init; }
    public required DateTime EventDate { get; init; }
    public required decimal Price { get; init; }
    public required int Seats { get; init; }
    public required int CourseEventTypeId { get; init; }
    public required VenueTypeRequest VenueType { get; init; }
}
