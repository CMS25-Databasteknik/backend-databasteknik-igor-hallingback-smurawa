namespace Backend.Domain.Modules.CourseRegistrationStatuses.Models;

public sealed class CourseRegistrationStatus
{
    public int Id { get; }
    public string Name { get; private set; } = string.Empty;

    public static CourseRegistrationStatus Pending { get; } = new(0, "Pending");
    public static CourseRegistrationStatus Paid { get; } = new(1, "Paid");
    public static CourseRegistrationStatus Cancelled { get; } = new(2, "Cancelled");
    public static CourseRegistrationStatus Refunded { get; } = new(3, "Refunded");

    public CourseRegistrationStatus(string name)
        : this(0, name)
    {
    }

    public CourseRegistrationStatus(int id, string name)
    {
        if (id < 0)
            throw new ArgumentException("Id must be zero or positive.", nameof(id));

        Id = id;
        SetValues(name);
    }

    public void Update(string name)
    {
        SetValues(name);
    }

    private void SetValues(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty or whitespace.", nameof(name));

        Name = name.Trim();
    }
}
