using Backend.Domain.Modules.Courses.Models;
using CourseWithEventsModel = Backend.Domain.Modules.CourseWithEvents.Models.CourseWithEvents;

namespace Backend.Domain.Modules.Courses.Contracts;

public interface ICourseRepository
{
    Task<Course> AddAsync(Course course, CancellationToken cancellationToken);
    Task<IReadOnlyList<CourseWithEventsModel>> GetAllAsync(CancellationToken cancellationToken);
    Task<CourseWithEventsModel?> GetByIdAsync(Guid courseId, CancellationToken cancellationToken);
    Task<Course?> UpdateAsync(Guid id, Course course, CancellationToken cancellationToken);
    Task<bool> RemoveAsync(Guid courseId, CancellationToken cancellationToken);
    Task<bool> HasCourseEventsAsync(Guid courseId, CancellationToken cancellationToken);
}

