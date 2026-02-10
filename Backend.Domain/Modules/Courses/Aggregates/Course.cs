namespace Backend.Domain.Modules.Courses.Aggregates;

public sealed class Course(Guid id, string title, string description, int durationInDays, DateTime createdAtUtc)
{
    public Guid Id { get; } = id;
    public string Title { get; } = title;
    public string Description { get; } = description;
    public int DurationInDays { get; } = durationInDays;
    public DateTime CreatedAtUtc { get; } = createdAtUtc;
    public static Course Create(Guid id, string title, string description, int durationInDays)
    {
        return new Course(id, title, description, durationInDays, DateTime.UtcNow);
    }
}
