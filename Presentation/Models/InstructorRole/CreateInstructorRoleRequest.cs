using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.InstructorRole;

public sealed record CreateInstructorRoleRequest
{
    [Required]
    public string RoleName { get; init; } = string.Empty;
}
