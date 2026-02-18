namespace Backend.Domain.Modules.CourseRegistrationStatuses.Models;

public sealed class CourseRegistrationStatus
{
    public int Id { get; }
    public string Name { get; }

    public static void ValidateId(int id)
    {
        if (id < 0)
            throw new ArgumentException("Id must be zero or positive.", nameof(id));
    }

    public static string ValidateAndNormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty or whitespace.", nameof(name));

        return name.Trim();
    }

    public CourseRegistrationStatus(int id, string name)
    {
        ValidateId(id);

        Id = id;
        Name = ValidateAndNormalizeName(name);
    }
}
