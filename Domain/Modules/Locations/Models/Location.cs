namespace Backend.Domain.Modules.Locations.Models;

public sealed class Location
{
    public int Id { get; }
    public string StreetName { get; }
    public string PostalCode { get; }
    public string City { get; }

    public Location(int id, string streetName, string postalCode, string city)
    {
        if (id < 0)
            throw new ArgumentException("ID must be greater than or equal to zero.", nameof(id));

        if (string.IsNullOrWhiteSpace(streetName))
            throw new ArgumentException("Street name cannot be null or whitespace.", nameof(streetName));

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be null or whitespace.", nameof(postalCode));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be null or whitespace.", nameof(city));

        Id = id;
        StreetName = streetName;
        PostalCode = postalCode;
        City = city;
    }
}
