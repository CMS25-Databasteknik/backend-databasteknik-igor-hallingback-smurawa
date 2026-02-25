using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.ParticipantContactType;

public sealed record CreateParticipantContactTypeRequest
{
    [Required]
    public required string Name { get; init; }
}
