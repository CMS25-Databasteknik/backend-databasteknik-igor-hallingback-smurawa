namespace Backend.Presentation.API.Models.Location;

public sealed record UpdateLocationRequest(
    string StreetName,
    string PostalCode,
    string City
);
