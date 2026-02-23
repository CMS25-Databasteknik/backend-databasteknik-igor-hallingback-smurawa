using Backend.Application.Modules.CourseEvents.Inputs;
using Backend.Application.Modules.CourseEvents.Outputs;

namespace Backend.Application.Modules.CourseEvents
{
    public interface ICourseEventService
    {
        Task<CourseEventResult> CreateCourseEventAsync(CreateCourseEventInput courseEvent, CancellationToken cancellationToken = default);
        Task<CourseEventListResult> GetAllCourseEventsAsync(CancellationToken cancellationToken = default);
        Task<CourseEventDetailsResult> GetCourseEventByIdAsync(Guid courseEventId, CancellationToken cancellationToken = default);
        Task<CourseEventListResult> GetCourseEventsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
        Task<CourseEventResult> UpdateCourseEventAsync(UpdateCourseEventInput courseEvent, CancellationToken cancellationToken = default);
        Task<CourseEventDeleteResult> DeleteCourseEventAsync(Guid courseEventId, CancellationToken cancellationToken = default);
    }
}
