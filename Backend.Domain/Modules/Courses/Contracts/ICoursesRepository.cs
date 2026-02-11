using Backend.Domain.Models.Course;
using Backend.Domain.Modules.Courses.Models;

namespace Backend.Application.Interfaces
{
    public interface ICoursesRepository
    {
        Task<Course> CreateCourseAsync(Course course, CancellationToken cancellationToken);
        Task<Course?> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken);
        Task<Course?> GetCourseByTitleAsync(string title, CancellationToken cancellationToken);
        Task<Course?> UpdateCourseAsync(UpdateCourseDto course, CancellationToken cancellationToken);
        Task<IReadOnlyList<Course>> GetAllCoursesAsync(CancellationToken cancellationToken);
        Task<bool> DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken);
    }
}
