using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.Participant;

public sealed record CreateParticipantRequest
{
    [Required]
    public required string FirstName { get; init; }

    [Required]
    public required string LastName { get; init; }

    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    public required string PhoneNumber { get; init; }

    [Range(1, int.MaxValue)]
    public required int ContactTypeId { get; init; }
}
