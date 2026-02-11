using Backend.Application.Modules.Courses.Inputs;
using Backend.Application.Modules.Courses.Outputs;
using Backend.Domain.Modules.Courses.Models;

namespace Backend.Application.Modules.Courses
{
    public interface ICourseService
    {
        Task<CourseResult> CreateCourseAsync(CreateCourseInput course, CancellationToken cancellationToken = default);
        Task<CourseListResult> GetAllCoursesAsync(CancellationToken cancellationToken = default);
        Task<Course?> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default);
        Task<Course?> GetCourseByTitleAsync(string title, CancellationToken cancellationToken = default);
        Task<Course?> UpdateCourseAsync(UpdateCourseInput course, CancellationToken cancellationToken = default);
        Task<CourseDeleteResult> DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken = default);

    }
}
