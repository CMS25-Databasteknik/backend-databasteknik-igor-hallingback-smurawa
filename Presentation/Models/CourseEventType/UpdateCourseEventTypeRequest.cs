using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.CourseEventType;

public sealed record UpdateCourseEventTypeRequest
{
    [Required]
    public required string TypeName { get; init; }
}
