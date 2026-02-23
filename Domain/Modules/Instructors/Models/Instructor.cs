using Backend.Domain.Modules.InstructorRoles.Models;
using System.Diagnostics.CodeAnalysis;

namespace Backend.Domain.Modules.Instructors.Models;

public sealed class Instructor
{
    public Guid Id { get; }
    public string Name { get; private set; } = string.Empty;
    public int InstructorRoleId { get; private set; }
    public InstructorRole Role { get; private set; }

    public Instructor(Guid id, string name, InstructorRole role)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID cannot be empty.", nameof(id));

        Id = id;
        SetValues(name, role);
    }

    public void Update(string name, InstructorRole role)
    {
        SetValues(name, role);
    }

    [MemberNotNull(nameof(Role))]
    private void SetValues(string name, InstructorRole role)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty or whitespace.", nameof(name));

        Role = role ?? throw new ArgumentNullException(nameof(role));

        if (role.Id <= 0)
            throw new ArgumentException("Instructor role ID must be greater than zero.", nameof(role));

        Name = name.Trim();
        Role = role;
        InstructorRoleId = role.Id;
    }
}
