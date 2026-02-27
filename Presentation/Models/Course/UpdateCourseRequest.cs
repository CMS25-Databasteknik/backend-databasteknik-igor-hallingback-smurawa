using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.Course;

public sealed record UpdateCourseRequest
{
    [Required]
    public string Title { get; init; }

    [Required]
    public string Description { get; init; }

    [Range(1, int.MaxValue)]
    public int DurationInDays { get; init; }
}
