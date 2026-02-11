using Backend.Domain.Modules.Courses.Models;

namespace Backend.Domain.Modules.Courses.Contracts
{
    public interface ICourseEventsRepository
    {
        Task<CourseEvent> CreateCourseEventAsync(CourseEvent courseEvent, CancellationToken cancellationToken);
        Task<CourseEvent?> GetCourseEventByIdAsync(Guid courseEventId, CancellationToken cancellationToken);
        Task<IReadOnlyList<CourseEvent>> GetAllCourseEventsAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<CourseEvent>> GetCourseEventsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken);
        Task<CourseEvent?> UpdateCourseEventAsync(CourseEvent courseEvent, CancellationToken cancellationToken);
        Task<bool> DeleteCourseEventAsync(Guid courseEventId, CancellationToken cancellationToken);
    }
}
