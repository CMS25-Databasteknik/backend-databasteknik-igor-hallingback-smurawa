using Backend.Domain.Modules.CourseRegistrations.Models;

namespace Backend.Presentation.API.Models.CourseRegistration;

public sealed record UpdateCourseRegistrationRequest(
    Guid ParticipantId,
    Guid CourseEventId,
    CourseRegistrationStatus Status
);
