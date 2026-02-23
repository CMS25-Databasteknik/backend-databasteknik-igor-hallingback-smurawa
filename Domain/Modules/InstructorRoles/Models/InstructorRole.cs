namespace Backend.Domain.Modules.InstructorRoles.Models;

public sealed class InstructorRole
{
    public int Id { get; }
    public string RoleName { get; private set; } = string.Empty;

    public InstructorRole(string roleName)
        : this(0, roleName)
    {
    }

    public InstructorRole(int id, string roleName)
    {
        if (id < 0)
            throw new ArgumentException("Id must be greater than or equal to zero.", nameof(id));

        Id = id;
        SetValues(roleName);
    }

    public void Update(string roleName)
    {
        SetValues(roleName);
    }

    private void SetValues(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("Role name cannot be empty or whitespace.", nameof(roleName));

        RoleName = roleName.Trim();
    }
}
