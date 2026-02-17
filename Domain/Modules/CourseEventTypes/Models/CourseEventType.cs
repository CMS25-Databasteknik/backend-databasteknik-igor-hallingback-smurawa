namespace Backend.Domain.Modules.CourseEventTypes.Models;

public sealed class CourseEventType
{
    public int Id { get; }
    public string TypeName { get; }

    public CourseEventType(int id, string typeName)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        if (string.IsNullOrWhiteSpace(typeName))
            throw new ArgumentException("Type name cannot be empty or whitespace.", nameof(typeName));

        Id = id;
        TypeName = typeName.Trim();
    }
}
