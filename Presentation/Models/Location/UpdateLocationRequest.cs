using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.Location;

public sealed record UpdateLocationRequest
{
    [Required]
    public required string StreetName { get; init; }

    [Required]
    public required string PostalCode { get; init; }

    [Required]
    public required string City { get; init; }
}
