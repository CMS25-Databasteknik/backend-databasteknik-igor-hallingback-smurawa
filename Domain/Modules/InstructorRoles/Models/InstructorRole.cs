using System.Text.Json.Serialization;

namespace Backend.Domain.Modules.InstructorRoles.Models;

public sealed class InstructorRole
{
    public int Id { get; }
    public string Name { get; private set; } = string.Empty;

    [JsonConstructor]
    private InstructorRole(int id, string name)
    {
        if (id < 0)
            throw new ArgumentException("Id must be greater than or equal to zero.", nameof(id));

        Id = id;
        SetValues(name);
    }

    public static InstructorRole Create(string name)
        => new(0, name);

    public static InstructorRole Reconstitute(int id, string name)
        => new(id, name);

    public void Update(string name)
    {
        SetValues(name);
    }

    private void SetValues(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty or whitespace.", nameof(name));

        Name = name.Trim();
    }
}
