using Backend.Domain.Modules.PaymentMethod.Models;

namespace Backend.Presentation.API.Models.CourseRegistration;

public sealed record UpdateCourseRegistrationRequest(
    Guid ParticipantId,
    Guid CourseEventId,
    int StatusId,
    PaymentMethod PaymentMethod
);

