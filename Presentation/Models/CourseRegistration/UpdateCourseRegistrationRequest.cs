namespace Backend.Presentation.API.Models.CourseRegistration;

public sealed class UpdateCourseRegistrationRequest
{
    public required Guid ParticipantId { get; init; }
    public required Guid CourseEventId { get; init; }
    public required int StatusId { get; init; }
    public required PaymentMethodRequest PaymentMethod { get; init; }
}

