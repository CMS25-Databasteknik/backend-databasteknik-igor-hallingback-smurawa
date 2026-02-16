namespace Backend.Presentation.API.Models.CourseRegistration;

public sealed record CreateCourseRegistrationRequest(
    Guid ParticipantId,
    Guid CourseEventId,
    bool IsPaid
);
