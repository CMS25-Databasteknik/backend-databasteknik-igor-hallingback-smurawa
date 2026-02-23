namespace Backend.Domain.Modules.Locations.Models;

public sealed class Location
{
    public int Id { get; }
    public string StreetName { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;

    public Location(int id, string streetName, string postalCode, string city)
    {
        if (id < 0)
            throw new ArgumentException("ID must be greater than or equal to zero.", nameof(id));

        Id = id;
        SetValues(streetName, postalCode, city);
    }

    public void Update(string streetName, string postalCode, string city)
    {
        SetValues(streetName, postalCode, city);
    }

    private void SetValues(string streetName, string postalCode, string city)
    {
        if (string.IsNullOrWhiteSpace(streetName))
            throw new ArgumentException("Street name cannot be empty or whitespace.", nameof(streetName));

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be empty or whitespace.", nameof(postalCode));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty or whitespace.", nameof(city));

        StreetName = streetName.Trim();
        PostalCode = postalCode.Trim();
        City = city.Trim();
    }
}
