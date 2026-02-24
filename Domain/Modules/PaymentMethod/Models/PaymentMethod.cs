namespace Backend.Domain.Modules.PaymentMethod.Models;

public sealed class PaymentMethod : IEquatable<PaymentMethod>
{
    public int Id { get; }
    public string Name { get; private set; } = string.Empty;

    public PaymentMethod(int id, string name)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(id);

        Id = id;
        SetValues(name);
    }

    public void Update(string name)
    {
        SetValues(name);
    }

    public override string ToString() => Name;

    public bool Equals(PaymentMethod? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return Id == other.Id && string.Equals(Name, other.Name, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) => obj is PaymentMethod other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Id, Name);

    private void SetValues(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Payment method name cannot be empty or whitespace.", nameof(name));

        Name = name.Trim();
    }
}
