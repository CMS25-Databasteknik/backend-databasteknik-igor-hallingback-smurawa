namespace Backend.Domain.Modules.VenueTypes.Models;

public sealed class VenueType : IEquatable<VenueType>
{
    public int Id { get; }
    public string Name { get; private set; } = string.Empty;

    public VenueType(int id, string name)
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

    public static implicit operator int(VenueType venueType) => venueType.Id;

    public bool Equals(VenueType? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return Id == other.Id && string.Equals(Name, other.Name, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) => obj is VenueType other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Id, Name);

    private void SetValues(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Venue type name cannot be empty or whitespace.", nameof(name));

        Name = name.Trim();
    }
}
