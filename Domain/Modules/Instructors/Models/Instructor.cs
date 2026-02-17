namespace Backend.Domain.Modules.Instructors.Models;

public sealed class Instructor
{
    public Guid Id { get; }
    public string Name { get; }
    public int InstructorRoleId { get; }
    public InstructorRole Role { get; }

    public Instructor(Guid id, string name, InstructorRole role)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID cannot be empty.", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

        Role = role ?? throw new ArgumentNullException(nameof(role));

        Id = id;
        Name = name;
        InstructorRoleId = role.Id;
    }
}
