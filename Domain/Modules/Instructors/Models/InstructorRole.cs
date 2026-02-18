namespace Backend.Domain.Modules.Instructors.Models;

public sealed class InstructorRole
{
    public InstructorRole(int id, string roleName)
    {
        if (id < 0)
            throw new ArgumentException("Id must be greater than or equal to zero.", nameof(id));

        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("Role name cannot be empty or whitespace.", nameof(roleName));

        Id = id;
        RoleName = roleName.Trim();
    }

    public int Id { get; }
    public string RoleName { get; }
}
