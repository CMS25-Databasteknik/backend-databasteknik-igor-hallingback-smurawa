namespace Backend.Domain.Modules.Courses.Models;

public sealed class Course
{
    public string Title { get; }
    public string Description { get; }
    public int DurationInDays { get; }

    public Course(string title, string description, int durationInDays)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Course title cannot be empty.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Course description cannot be empty.", nameof(description));

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(durationInDays);

        Title = title.Trim();
        Description = description.Trim();
        DurationInDays = durationInDays;
    }
}