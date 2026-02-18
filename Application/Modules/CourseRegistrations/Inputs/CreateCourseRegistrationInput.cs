using Backend.Domain.Modules.CourseRegistrationStatuses.Models;
using Backend.Domain.Modules.PaymentMethod.Models;

namespace Backend.Application.Modules.CourseRegistrations.Inputs;

public sealed record CreateCourseRegistrationInput(
    Guid ParticipantId,
    Guid CourseEventId,
    CourseRegistrationStatus Status,
    PaymentMethod PaymentMethod
);

