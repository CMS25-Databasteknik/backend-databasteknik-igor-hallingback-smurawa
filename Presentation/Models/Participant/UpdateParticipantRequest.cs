using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.Participant;

public sealed record UpdateParticipantRequest
{
    [Required]
    public string FirstName { get; init; }

    [Required]
    public string LastName { get; init; }

    [Required]
    [EmailAddress]
    public string Email { get; init; }

    [Required]
    public string PhoneNumber { get; init; }

    [Range(1, int.MaxValue)]
    public int ContactTypeId { get; init; }
}
