using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.Location;

public sealed record CreateLocationRequest
{
    [Required]
    public string StreetName { get; init; } = string.Empty;

    [Required]
    public string PostalCode { get; init; } = string.Empty;

    [Required]
    public string City { get; init; } = string.Empty;
}
