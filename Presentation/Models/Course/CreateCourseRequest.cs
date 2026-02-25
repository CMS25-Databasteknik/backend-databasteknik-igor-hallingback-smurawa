using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.Course;

public sealed record CreateCourseRequest
{
    [Required]
    public required string Title { get; init; }

    [Required]
    public required string Description { get; init; }

    [Range(1, int.MaxValue)]
    public required int DurationInDays { get; init; }
}
