using Backend.Domain.Modules.CourseRegistrations.Models;

namespace Backend.Application.Modules.CourseRegistrations.Inputs;

public sealed record UpdateCourseRegistrationInput(
    Guid Id,
    Guid ParticipantId,
    Guid CourseEventId,
    CourseRegistrationStatus Status
);
