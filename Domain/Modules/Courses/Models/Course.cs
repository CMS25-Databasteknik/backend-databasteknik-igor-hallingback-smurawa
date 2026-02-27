namespace Backend.Domain.Modules.Courses.Models;

public sealed class Course
{
    public Guid Id { get; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int DurationInDays { get; private set; }

    public Course(
        Guid id,
        string title,
        string description,
        int durationInDays)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Course id cannot be empty.", nameof(id));

        Id = id;
        SetValues(title, description, durationInDays);
    }

    public void Update(string title, string description, int durationInDays)
    {
        SetValues(title, description, durationInDays);
    }

    private void SetValues(string title, string description, int durationInDays)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Course title cannot be empty or whitespace.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Course description cannot be empty or whitespace.", nameof(description));

        if (durationInDays <= 0)
            throw new ArgumentOutOfRangeException(nameof(durationInDays), "Course duration must be greater than zero.");

        Title = title.Trim();
        Description = description.Trim();
        DurationInDays = durationInDays;
    }
}
