using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.InstructorRole;

public sealed record UpdateInstructorRoleRequest
{
    [Required]
    public required string RoleName { get; init; }
}
