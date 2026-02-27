using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.Participant;

public sealed record CreateParticipantRequest
{
    [Required]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    public string LastName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string PhoneNumber { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int ContactTypeId { get; init; }
}
