using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.CourseRegistration;

public sealed record CreateCourseRegistrationRequest
{
    public required Guid ParticipantId { get; init; }

    public required Guid CourseEventId { get; init; }

    [Range(0, int.MaxValue)]
    public required int StatusId { get; init; }

    [Range(0, int.MaxValue)]
    public required int PaymentMethodId { get; init; }
}
