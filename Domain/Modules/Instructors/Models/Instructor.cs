namespace Backend.Domain.Modules.Instructors.Models;

public sealed class Instructor
{
    public Guid Id { get; }
    public string Name { get; }
    public int InstructorRoleId { get; }

    public Instructor(Guid id, string name, int instructorRoleId = 1)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID cannot be empty.", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

        if (instructorRoleId < 1)
            throw new ArgumentException("Instructor role is required.", nameof(instructorRoleId));

        Id = id;
        Name = name;
        InstructorRoleId = instructorRoleId;
    }
}
