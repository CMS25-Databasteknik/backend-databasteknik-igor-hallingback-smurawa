using Backend.Domain.Modules.CourseEventTypes.Models;
using Backend.Domain.Modules.VenueTypes.Models;
using System.Diagnostics.CodeAnalysis;

namespace Backend.Domain.Modules.CourseEvents.Models;

public sealed class CourseEvent
{
    public Guid Id { get; }
    public Guid CourseId { get; private set; }
    public DateTime EventDate { get; private set; }
    public decimal Price { get; private set; }
    public int Seats { get; private set; }
    public int CourseEventTypeId { get; private set; }
    public int VenueTypeId { get; private set; }
    public CourseEventType CourseEventType { get; private set; }
    public VenueType VenueType { get; private set; }

    public CourseEvent(
        Guid id,
        Guid courseId,
        DateTime eventDate,
        decimal price,
        int seats,
        int courseEventTypeId,
        VenueType venueType,
        CourseEventType? courseEventType = null,
        VenueType? resolvedVenueType = null)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("CourseEvent id cannot be empty.", nameof(id));

        Id = id;
        SetValues(courseId, eventDate, price, seats, courseEventTypeId, venueType, courseEventType, resolvedVenueType);
    }

    public void Update(
        Guid courseId,
        DateTime eventDate,
        decimal price,
        int seats,
        int courseEventTypeId,
        VenueType venueType,
        CourseEventType? courseEventType = null,
        VenueType? resolvedVenueType = null)
    {
        SetValues(courseId, eventDate, price, seats, courseEventTypeId, venueType, courseEventType, resolvedVenueType);
    }

    [MemberNotNull(nameof(CourseEventType), nameof(VenueType))]
    private void SetValues(
        Guid courseId,
        DateTime eventDate,
        decimal price,
        int seats,
        int courseEventTypeId,
        VenueType venueType,
        CourseEventType? courseEventType,
        VenueType? resolvedVenueType)
    {
        if (courseId == Guid.Empty)
            throw new ArgumentException("Course id cannot be empty.", nameof(courseId));

        if (eventDate == default)
            throw new ArgumentException("Event date must be specified.", nameof(eventDate));

        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(seats);

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(courseEventTypeId);
        ArgumentNullException.ThrowIfNull(venueType);

        if (courseEventType is not null && courseEventType.Id != courseEventTypeId)
            throw new ArgumentException("Course event type ID mismatch.", nameof(courseEventType));
        if (resolvedVenueType is not null && resolvedVenueType.Id != venueType.Id)
            throw new ArgumentException("Venue type ID mismatch.", nameof(resolvedVenueType));

        CourseId = courseId;
        EventDate = eventDate;
        Price = price;
        Seats = seats;
        CourseEventTypeId = courseEventTypeId;
        VenueTypeId = venueType.Id;
        CourseEventType = courseEventType ?? new CourseEventType(courseEventTypeId, $"Type {courseEventTypeId}");
        VenueType = resolvedVenueType ?? new VenueType(venueType.Id, venueType.Name);
    }
}
