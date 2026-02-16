using Backend.Application.Modules.CourseRegistrations.Inputs;
using Backend.Application.Modules.CourseRegistrations.Outputs;

namespace Backend.Application.Modules.CourseRegistrations;

public interface ICourseRegistrationService
{
    Task<CourseRegistrationResult> CreateCourseRegistrationAsync(CreateCourseRegistrationInput courseRegistration, CancellationToken cancellationToken = default);
    Task<CourseRegistrationListResult> GetAllCourseRegistrationsAsync(CancellationToken cancellationToken = default);
    Task<CourseRegistrationResult> GetCourseRegistrationByIdAsync(Guid courseRegistrationId, CancellationToken cancellationToken = default);
    Task<CourseRegistrationListResult> GetCourseRegistrationsByParticipantIdAsync(Guid participantId, CancellationToken cancellationToken = default);
    Task<CourseRegistrationListResult> GetCourseRegistrationsByCourseEventIdAsync(Guid courseEventId, CancellationToken cancellationToken = default);
    Task<CourseRegistrationResult> UpdateCourseRegistrationAsync(UpdateCourseRegistrationInput courseRegistration, CancellationToken cancellationToken = default);
    Task<CourseRegistrationDeleteResult> DeleteCourseRegistrationAsync(Guid courseRegistrationId, CancellationToken cancellationToken = default);
}
