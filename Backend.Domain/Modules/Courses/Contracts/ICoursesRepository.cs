using Backend.Domain.Modules.Courses.Models;
using Backend.Domain.Modules.CourseEvents.Models;

namespace Backend.Domain.Modules.Courses.Contracts
{
    public interface ICoursesRepository
    {
        Task<Course> CreateCourseAsync(Course course, CancellationToken cancellationToken);
        Task<(Course Course, IReadOnlyList<CourseEvent> Events)?> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken);
        Task<Course?> UpdateCourseAsync(Course course, CancellationToken cancellationToken);
        Task<IReadOnlyList<Course>> GetAllCoursesAsync(CancellationToken cancellationToken);
        Task<bool> DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken);
    }
}
