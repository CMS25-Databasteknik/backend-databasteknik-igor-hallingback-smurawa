namespace Backend.Domain.Modules.Instructors.Models;

public sealed class Instructor
{
    public Guid Id { get; }
    public string Name { get; }

    public Instructor(Guid id, string name)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID cannot be empty.", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

        Id = id;
        Name = name;
    }
}
