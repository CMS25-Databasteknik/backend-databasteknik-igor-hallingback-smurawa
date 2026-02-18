using Backend.Domain.Modules.Courses.Models;
using CourseWithEventsModel = Backend.Domain.Modules.CourseWithEvents.Models.CourseWithEvents;

namespace Backend.Domain.Modules.Courses.Contracts;

public interface ICourseRepository
{
    Task<Course> CreateCourseAsync(Course course, CancellationToken cancellationToken);
    Task<CourseWithEventsModel?> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken);
    Task<Course?> UpdateCourseAsync(Course course, CancellationToken cancellationToken);
    Task<IReadOnlyList<Course>> GetAllCoursesAsync(CancellationToken cancellationToken);
    Task<bool> DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken);
    Task<bool> HasCourseEventsAsync(Guid courseId, CancellationToken cancellationToken);
}
