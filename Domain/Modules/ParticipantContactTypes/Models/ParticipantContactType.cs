namespace Backend.Domain.Modules.ParticipantContactTypes.Models;

public sealed class ParticipantContactType : IEquatable<ParticipantContactType>
{
    public int Id { get; }
    public string Name { get; private set; } = string.Empty;

    public ParticipantContactType(int id, string name)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        Id = id;
        SetValues(name);
    }

    public void Update(string name)
    {
        SetValues(name);
    }

    public override string ToString() => Name;

    public bool Equals(ParticipantContactType? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return Id == other.Id && string.Equals(Name, other.Name, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) => obj is ParticipantContactType other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Id, Name);

    private void SetValues(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Participant contact type name cannot be empty or whitespace.", nameof(name));

        Name = name.Trim();
    }
}
