using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.CourseEventType;

public sealed record UpdateCourseEventTypeRequest
{
    [Required]
    public string TypeName { get; init; } = string.Empty;
}
