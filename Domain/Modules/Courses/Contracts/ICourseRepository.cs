using Backend.Domain.Modules.Courses.Models;
using Backend.Domain.Common.Base;
using CourseWithEventsModel = Backend.Domain.Modules.CourseWithEvents.Models.CourseWithEvents;

namespace Backend.Domain.Modules.Courses.Contracts;

public interface ICourseRepository : IRepositoryBase<Course, Guid>
{
    Task<CourseWithEventsModel?> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken);
    Task<bool> HasCourseEventsAsync(Guid courseId, CancellationToken cancellationToken);
}
