using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.ParticipantContactType;

public sealed record UpdateParticipantContactTypeRequest
{
    [Required]
    public required string Name { get; init; }
}
