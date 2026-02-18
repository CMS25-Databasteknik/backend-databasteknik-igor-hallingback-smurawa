using Backend.Domain.Modules.CourseRegistrationStatuses.Models;

namespace Backend.Domain.Modules.CourseRegistrations.Contracts;

public interface ICourseRegistrationStatusRepository
{
    Task<CourseRegistrationStatus> CreateCourseRegistrationStatusAsync(CourseRegistrationStatus status, CancellationToken cancellationToken);
    Task<IReadOnlyList<CourseRegistrationStatus>> GetAllCourseRegistrationStatusesAsync(CancellationToken cancellationToken);
    Task<CourseRegistrationStatus?> GetCourseRegistrationStatusByIdAsync(int statusId, CancellationToken cancellationToken);
    Task<CourseRegistrationStatus?> UpdateCourseRegistrationStatusAsync(CourseRegistrationStatus status, CancellationToken cancellationToken);
    Task<bool> DeleteCourseRegistrationStatusAsync(int statusId, CancellationToken cancellationToken);
    Task<bool> IsInUseAsync(int statusId, CancellationToken cancellationToken);
}
