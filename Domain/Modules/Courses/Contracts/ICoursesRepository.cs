using Backend.Domain.Modules.Courses.Models;

namespace Backend.Domain.Modules.Courses.Contracts
{
    public interface ICoursesRepository
    {
        Task<Course> CreateCourseAsync(Course course, CancellationToken cancellationToken);
        Task<CourseWithEvents?> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken);
        Task<Course?> UpdateCourseAsync(Course course, CancellationToken cancellationToken);
        Task<IReadOnlyList<Course>> GetAllCoursesAsync(CancellationToken cancellationToken);
        Task<bool> DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken);
        Task<bool> HasCourseEventsAsync(Guid courseId, CancellationToken cancellationToken);
    }
}
