namespace Backend.Domain.Modules.Courses.Models;

public sealed class Course
{
    public Guid Id { get; }
    public string Title { get; }
    public string Description { get; }
    public int DurationInDays { get; }

    public Course(
        Guid id,
        string title,
        string description,
        int durationInDays)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Course id cannot be empty.", nameof(id));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Course title cannot be empty or whitespace.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Course description cannot be empty or whitespace.", nameof(description));

        if (durationInDays <= 0)
            throw new ArgumentOutOfRangeException(nameof(durationInDays), "Course duration must be greater than zero.");

        Id = id;
        Title = title.Trim();
        Description = description.Trim();
        DurationInDays = durationInDays;
    }
}
