namespace Backend.Domain.Modules.Instructors.Models;

public sealed class InstructorRole
{
    public InstructorRole(int id, string roleName)
    {
        if (id < 1)
            throw new ArgumentException("Id is required.", nameof(id));

        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("RoleName is required.", nameof(roleName));

        Id = id;
        RoleName = roleName.Trim();
    }

    public int Id { get; }
    public string RoleName { get; }
}
