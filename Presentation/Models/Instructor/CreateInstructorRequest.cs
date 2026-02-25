using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.Instructor;

public sealed record CreateInstructorRequest
{
    [Required]
    public required string Name { get; init; }

    [Range(1, int.MaxValue)]
    public required int InstructorRoleId { get; init; }
}
