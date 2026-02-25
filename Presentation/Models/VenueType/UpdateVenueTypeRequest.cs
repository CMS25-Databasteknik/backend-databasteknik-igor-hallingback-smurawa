using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.VenueType;

public sealed record UpdateVenueTypeRequest
{
    [Required]
    public required string Name { get; init; }
}
