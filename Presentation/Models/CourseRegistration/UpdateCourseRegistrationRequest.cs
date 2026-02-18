using Backend.Domain.Modules.CourseRegistrationStatuses.Models;
using Backend.Domain.Modules.PaymentMethod.Models;

namespace Backend.Presentation.API.Models.CourseRegistration;

public sealed record UpdateCourseRegistrationRequest(
    Guid ParticipantId,
    Guid CourseEventId,
    CourseRegistrationStatus Status,
    PaymentMethod PaymentMethod
);

