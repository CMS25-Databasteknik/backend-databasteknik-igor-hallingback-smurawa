using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.Instructor;

public sealed record CreateInstructorRequest
{
    [Required]
    public string Name { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int InstructorRoleId { get; init; }
}
