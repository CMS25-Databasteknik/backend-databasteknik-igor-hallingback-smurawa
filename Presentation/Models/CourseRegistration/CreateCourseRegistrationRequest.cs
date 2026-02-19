using Backend.Domain.Modules.PaymentMethod.Models;

namespace Backend.Presentation.API.Models.CourseRegistration;

public sealed record CreateCourseRegistrationRequest(
    Guid ParticipantId,
    Guid CourseEventId,
    int StatusId,
    PaymentMethod PaymentMethod
);

