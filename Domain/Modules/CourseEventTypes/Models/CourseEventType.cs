namespace Backend.Domain.Modules.CourseEventTypes.Models;

public sealed class CourseEventType
{
    public int Id { get; }
    public string TypeName { get; private set; } = string.Empty;

    public CourseEventType(string typeName)
        : this(0, typeName)
    {
    }

    public CourseEventType(int id, string typeName)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(id);

        Id = id;
        SetValues(typeName);
    }

    public void Update(string typeName)
    {
        SetValues(typeName);
    }

    private void SetValues(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            throw new ArgumentException("Type name cannot be empty or whitespace.", nameof(typeName));

        TypeName = typeName.Trim();
    }
}
