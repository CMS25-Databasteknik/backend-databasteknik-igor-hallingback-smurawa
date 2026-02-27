using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.Course;

public sealed record CreateCourseRequest
{
    [Required]
    public string Title { get; init; } = string.Empty;

    [Required]
    public string Description { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int DurationInDays { get; init; }
}
