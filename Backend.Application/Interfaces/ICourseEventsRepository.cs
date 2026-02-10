using Backend.Domain.Models.CourseEvent;

namespace Backend.Application.Interfaces
{
    public interface ICourseEventsRepository
    {
        Task<CourseEventSummaryDto> CreateCourseEventAsync(CreateCourseEventDto courseEvent, CancellationToken cancellationToken);
        Task<CourseEventDto?> GetCourseEventByIdAsync(Guid courseEventId, CancellationToken cancellationToken);
        Task<IEnumerable<CourseEventSummaryDto>> GetAllCourseEventsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<CourseEventSummaryDto>> GetCourseEventsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken);
        Task<CourseEventSummaryDto?> UpdateCourseEventAsync(UpdateCourseEventDto courseEvent, CancellationToken cancellationToken);
        Task<bool> DeleteCourseEventAsync(Guid courseEventId, CancellationToken cancellationToken);
    }
}
