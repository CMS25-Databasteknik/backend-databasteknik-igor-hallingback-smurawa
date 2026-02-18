using Backend.Domain.Modules.CourseEvents.Models;

namespace Backend.Domain.Modules.CourseEvents.Contracts
{
    public interface ICourseEventRepository
    {
        Task<CourseEvent> CreateCourseEventAsync(CourseEvent courseEvent, CancellationToken cancellationToken);
        Task<CourseEvent?> GetCourseEventByIdAsync(Guid courseEventId, CancellationToken cancellationToken);
        Task<IReadOnlyList<CourseEvent>> GetAllCourseEventsAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<CourseEvent>> GetCourseEventsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken);
        Task<CourseEvent?> UpdateCourseEventAsync(CourseEvent courseEvent, CancellationToken cancellationToken);
        Task<bool> DeleteCourseEventAsync(Guid courseEventId, CancellationToken cancellationToken);
        Task<bool> HasRegistrationsAsync(Guid courseEventId, CancellationToken cancellationToken);
    }
}

