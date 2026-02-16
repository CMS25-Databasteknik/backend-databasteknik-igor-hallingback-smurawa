namespace Backend.Presentation.API.Models.Location;

public sealed record CreateLocationRequest(
    string StreetName,
    string PostalCode,
    string City
);
