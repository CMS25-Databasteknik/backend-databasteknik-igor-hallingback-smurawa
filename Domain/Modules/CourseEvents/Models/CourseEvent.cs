using Backend.Domain.Modules.CourseEventTypes.Models;
using Backend.Domain.Modules.VenueTypes.Models;

namespace Backend.Domain.Modules.CourseEvents.Models;

public sealed class CourseEvent
{
    public Guid Id { get; }
    public Guid CourseId { get; }
    public DateTime EventDate { get; }
    public decimal Price { get; }
    public int Seats { get; }
    public int CourseEventTypeId { get; }
    public CourseEventType CourseEventType { get; }
    public VenueType VenueType { get; }
    public string VenueTypeName { get; }

    public CourseEvent(
        Guid id,
        Guid courseId,
        DateTime eventDate,
        decimal price,
        int seats,
        int courseEventTypeId,
        VenueType venueType,
        CourseEventType? courseEventType = null,
        string? venueTypeName = null)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("CourseEvent id cannot be empty.", nameof(id));

        if (courseId == Guid.Empty)
            throw new ArgumentException("Course id cannot be empty.", nameof(courseId));

        if (eventDate == default)
            throw new ArgumentException("Event date must be specified.", nameof(eventDate));

        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(seats);

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(courseEventTypeId);
        if (!Enum.IsDefined(typeof(VenueType), venueType))
            throw new ArgumentException("Venue type is invalid.", nameof(venueType));

        if (courseEventType is not null && courseEventType.Id != courseEventTypeId)
            throw new ArgumentException("Course event type ID mismatch.", nameof(courseEventType));

        Id = id;
        CourseId = courseId;
        EventDate = eventDate;
        Price = price;
        Seats = seats;
        CourseEventTypeId = courseEventTypeId;
        CourseEventType = courseEventType ?? new CourseEventType(courseEventTypeId, $"Type {courseEventTypeId}");
        VenueType = venueType;
        VenueTypeName = string.IsNullOrWhiteSpace(venueTypeName)
            ? venueType.ToString()
            : venueTypeName.Trim();
    }
}
