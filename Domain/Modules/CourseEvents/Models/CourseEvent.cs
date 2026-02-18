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
    public VenueType VenueType { get; }

    public CourseEvent(
        Guid id,
        Guid courseId,
        DateTime eventDate,
        decimal price,
        int seats,
        int courseEventTypeId,
        VenueType venueType)
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

        Id = id;
        CourseId = courseId;
        EventDate = eventDate;
        Price = price;
        Seats = seats;
        CourseEventTypeId = courseEventTypeId;
        VenueType = venueType;
    }
}
