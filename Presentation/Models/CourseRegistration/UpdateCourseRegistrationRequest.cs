namespace Backend.Presentation.API.Models.CourseRegistration;

public sealed record UpdateCourseRegistrationRequest(
    Guid ParticipantId,
    Guid CourseEventId,
    bool IsPaid
);
