using Backend.Domain.Modules.CourseRegistrations.Models;

namespace Backend.Domain.Modules.CourseRegistrations.Contracts;

public interface ICourseRegistrationRepository
{
    Task<CourseRegistration> CreateCourseRegistrationAsync(CourseRegistration courseRegistration, CancellationToken cancellationToken);
    Task<CourseRegistration?> GetCourseRegistrationByIdAsync(Guid courseRegistrationId, CancellationToken cancellationToken);
    Task<IReadOnlyList<CourseRegistration>> GetAllCourseRegistrationsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<CourseRegistration>> GetCourseRegistrationsByParticipantIdAsync(Guid participantId, CancellationToken cancellationToken);
    Task<IReadOnlyList<CourseRegistration>> GetCourseRegistrationsByCourseEventIdAsync(Guid courseEventId, CancellationToken cancellationToken);
    Task<CourseRegistration?> UpdateCourseRegistrationAsync(CourseRegistration courseRegistration, CancellationToken cancellationToken);
    Task<bool> DeleteCourseRegistrationAsync(Guid courseRegistrationId, CancellationToken cancellationToken);
}

