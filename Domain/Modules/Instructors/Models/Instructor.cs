using Backend.Domain.Modules.InstructorRoles.Models;

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
            throw new ArgumentException("Name cannot be empty or whitespace.", nameof(name));

        Role = role ?? throw new ArgumentNullException(nameof(role));

        if (role.Id <= 0)
            throw new ArgumentException("Instructor role ID must be greater than zero.", nameof(role));

        Id = id;
        Name = name.Trim();
        InstructorRoleId = role.Id;
    }
}
