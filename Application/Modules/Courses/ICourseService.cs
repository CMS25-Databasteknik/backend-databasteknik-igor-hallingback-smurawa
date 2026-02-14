using Backend.Application.Modules.Courses.Inputs;
using Backend.Application.Modules.Courses.Outputs;

namespace Backend.Application.Modules.Courses
{
    public interface ICourseService
    {
        Task<CourseResult> CreateCourseAsync(CreateCourseInput course, CancellationToken cancellationToken = default);
        Task<CourseListResult> GetAllCoursesAsync(CancellationToken cancellationToken = default);
        Task<CourseWithEventsResult> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default);
        Task<CourseResult> UpdateCourseAsync(UpdateCourseInput course, CancellationToken cancellationToken = default);
        Task<CourseDeleteResult> DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken = default);

    }
}
