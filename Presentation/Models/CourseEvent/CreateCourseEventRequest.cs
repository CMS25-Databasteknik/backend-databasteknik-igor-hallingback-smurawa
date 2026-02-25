using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.CourseEvent;

public sealed record CreateCourseEventRequest
{
    public required Guid CourseId { get; init; }

    public required DateTime EventDate { get; init; }

    public required decimal Price { get; init; }

    [Range(1, int.MaxValue)]
    public required int Seats { get; init; }

    [Range(1, int.MaxValue)]
    public required int CourseEventTypeId { get; init; }

    [Range(1, int.MaxValue)]
    public required int VenueTypeId { get; init; }
}
