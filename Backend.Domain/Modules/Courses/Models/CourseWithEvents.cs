namespace Backend.Domain.Modules.Courses.Models;

public sealed class CourseWithEvents
{
    public Guid Id { get; }
    public string Title { get; }
    public string Description { get; }
    public int DurationInDays { get; }
    public IReadOnlyList<CourseEvent> CourseEvents { get; }

    public CourseWithEvents(
        Guid id,
        string title,
        string description,
        int durationInDays,
        IEnumerable<CourseEvent> courseEvents)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Course id cannot be empty.", nameof(id));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Course title cannot be empty.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Course description cannot be empty.", nameof(description));

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(durationInDays);

        Id = id;
        Title = title.Trim();
        Description = description.Trim();
        DurationInDays = durationInDays;
        CourseEvents = courseEvents?.ToList() ?? [];
    }
}