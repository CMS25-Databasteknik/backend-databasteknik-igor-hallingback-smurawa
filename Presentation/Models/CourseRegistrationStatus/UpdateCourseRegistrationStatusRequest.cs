using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.CourseRegistrationStatus;

public sealed record UpdateCourseRegistrationStatusRequest
{
    [Required]
    public required string Name { get; init; }
}
