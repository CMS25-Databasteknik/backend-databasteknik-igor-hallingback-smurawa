using Backend.Domain.Modules.CourseRegistrations.Models;

namespace Backend.Application.Modules.CourseRegistrations.Inputs;

public sealed record CreateCourseRegistrationInput(
    Guid ParticipantId,
    Guid CourseEventId,
    CourseRegistrationStatus Status
);
